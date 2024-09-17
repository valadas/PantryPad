using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Docker;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Utilities.Collections;
using Octokit;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

[GitHubActions(
    "build",
    GitHubActionsImage.UbuntuLatest,
    EnableGitHubToken = true,
    OnPullRequestBranches = ["main", "develop", "release/*"],
    OnPushBranches = ["main", "develop", "release/*"],
    InvokedTargets = [nameof(CI)],
    FetchDepth = 0,
    CacheKeyFiles = [])]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("GitHub Token for authentication")] readonly string GitHubToken;

    [Solution] readonly Solution Solution;
    [GitVersion] readonly GitVersion GitVersion;
    [GitRepository] readonly GitRepository GitRepository;

    Nuke.Common.ProjectModel.Project PantryPadProject => Solution.GetProject("PantryPad");
    static GitHubActions GitHubActions => GitHubActions.Instance;

    // PATHS
    AbsolutePath SourceDirectory => RootDirectory / "PantryPad";
    AbsolutePath WwwRootDirectory => SourceDirectory / "wwwroot";

    Target Compile => _ => _
        .DependsOn(BuildFrontEnd)
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild(s => s
                .SetProjectFile(PantryPadProject)
                .SetConfiguration(Configuration));
        });
    
    Target BuildFrontEnd => _ => _
        .Executes(() =>
        {
            NpmTasks.NpmInstall(s => s
                .SetProcessWorkingDirectory(WwwRootDirectory));
            NpmTasks.NpmRun(s => s
                .SetProcessWorkingDirectory(WwwRootDirectory)
                .SetCommand("build"));
        });

    Target DockerBuild => _ => _
        .DependsOn(BuildFrontEnd)
        .Executes(() =>
        {
            var owner = GitRepository.GetGitHubOwner();
            var buildResult = DockerTasks.DockerBuild(s => s
            .SetPath(RootDirectory)
            .SetFile(RootDirectory / "Dockerfile")
            .SetTag($"ghcr.io/{owner}/pantrypad:{GitVersion.SemVer}")
            .SetProcessLogger((type, output) => {
                if (output.Contains("ERROR:"))
                {
                    Serilog.Log.Error(output);
                }
                else
                {
                    Serilog.Log.Information(output);
                }
            }));
        });

    Target CI => _ => _
        .DependsOn(DockerBuild)
        .DependsOn(Release);

    Target Release => _ => _
        .OnlyWhenDynamic(() => GitRepository != null && (GitRepository.IsOnMainOrMasterBranch() || GitRepository.IsOnReleaseBranch()))
        .OnlyWhenDynamic(() => !string.IsNullOrWhiteSpace(GitHubToken))
        .DependsOn(DockerBuild)
        .Executes(async () =>
        {
            var credentials = new Credentials(GitHubActions.Token);
            GitHubTasks.GitHubClient = new GitHubClient(new ProductHeaderValue("Eraware.Dnn.Templates"))
            {
                Credentials = credentials,
            };
            var (owner, name) = (GitRepository.GetGitHubOwner(), GitRepository.GetGitHubName());
            var version = GitRepository.IsOnMainOrMasterBranch() ? GitVersion.MajorMinorPatch : GitVersion.FullSemVer;
            var releaseNotes = GetReleaseNotes();
            var newRelease = new NewRelease(GitVersion.SemVer)
            {
                Draft = true,
                Name = $"v{version}",
                Body = releaseNotes,
                TargetCommitish = GitVersion.Sha,
                Prerelease = GitRepository.IsOnReleaseBranch(),
            };

            var createdRelease = await GitHubTasks
                .GitHubClient
                .Repository
                .Release
                .Create(owner, name, newRelease);

            // Publish container
            DockerTasks.DockerLogin(s => s
                .SetServer("ghcr.io")
                .SetUsername(GitHubActions.Token)
                .SetPassword(GitHubActions.Token)
                .SetProcessLogger((type, output) =>
                {
                    if (output.Contains("ERROR:"))
                    {
                        Serilog.Log.Error(output);
                    }
                    else
                    {
                        Serilog.Log.Information(output);
                    }
                }));
            DockerTasks.DockerPush(s => s
                .SetName($"ghcr.io/{owner}/pantrypad:{GitVersion.SemVer}")
                .SetProcessLogger((type, output) =>
                {
                    if (output.Contains("ERROR:"))
                    {
                        Serilog.Log.Error(output);
                    }
                    else
                    {
                        Serilog.Log.Information(output);
                    }
                }));
        });

    private string GetReleaseNotes()
    {
        var owner = GitRepository.GetGitHubOwner();
        var name = GitRepository.GetGitHubName();

        // Get the milestone
        var milestone = GitHubTasks.GitHubClient.Issue.Milestone.GetAllForRepository(owner, name).Result.Where(m => m.Title == GitVersion.MajorMinorPatch).FirstOrDefault();
        if (milestone == null)
        {
            Serilog.Log.Warning("Milestone not found for this version");
            return "No release notes for this version.";
        }

        // Get the PRs
        var prRequest = new PullRequestRequest()
        {
            State = ItemStateFilter.All
        };
        var pullRequests = GitHubTasks.GitHubClient.Repository.PullRequest.GetAllForRepository(owner, name, prRequest).Result.Where(p =>
            p.Milestone?.Title == milestone.Title &&
            p.Merged == true &&
            p.Milestone?.Title == GitVersion.MajorMinorPatch);

        // Build release notes
        var releaseNotesBuilder = new StringBuilder();
        releaseNotesBuilder.AppendLine($"# {name} {milestone.Title}")
            .AppendLine("")
            .AppendLine($"A total of {pullRequests.Count()} pull requests where merged in this release.").AppendLine();

        foreach (var group in pullRequests.GroupBy(p => p.Labels[0]?.Name, (label, prs) => new { label, prs }))
        {
            releaseNotesBuilder.AppendLine($"## {group.label}");
            foreach (var pr in group.prs)
            {
                releaseNotesBuilder.AppendLine($"- #{pr.Number} {pr.Title}. Thanks @{pr.User.Login}");
            }
        }

        var result = releaseNotesBuilder.ToString();
        Serilog.Log.Information(result);
        return result;
    }
}
