using System;
using System.Linq;
using System.Threading.Tasks;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Docker;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

[GitHubActions(
    "build",
    GitHubActionsImage.UbuntuLatest,
    On = new[] { GitHubActionsTrigger.Push },
    InvokedTargets = [nameof(CI)],
    CacheKeyFiles = [],
    FetchDepth = 0)]
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

    Project PantryPadProject => Solution.GetProject("PantryPad");

    // PATHS
    AbsolutePath SourceDirectory => RootDirectory / "PantryPad";
    AbsolutePath WwwRootDirectory => SourceDirectory / "wwwroot";

    Target Compile => _ => _
        .Executes(() =>
        {
            NpmTasks.NpmRun(s => s
                .SetProcessWorkingDirectory(WwwRootDirectory)
                .SetCommand("build"));
            NpmTasks.NpmInstall(s => s
                .SetProcessWorkingDirectory(WwwRootDirectory));
            DotNetTasks.DotNetBuild(s => s
                .SetProjectFile(PantryPadProject)
                .SetConfiguration(Configuration));
        });
    
    Target DockerBuild => _ => _
        .Executes(() =>
        {
            DockerTasks.DockerBuild(s => s
                .SetPath(RootDirectory)
                .SetFile(RootDirectory / "Dockerfile")
                .SetTag($"pantrypad:{GitVersion.MajorMinorPatch}.{GitVersion.CommitsSinceVersionSource}"));
        });

    Target CI => _ => _
        .DependsOn(DockerBuild);
}
