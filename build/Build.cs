using System;
using System.Linq;
using System.Threading.Tasks;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Run);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;

    Target Run => _ => _
        .Executes(async () =>
        {
            var dotnetRunTask = Task.Run(() => DotNetRun());
            var npmStartTask = Task.Run(() => NpmStart());

            await Task.WhenAll(dotnetRunTask, npmStartTask);
        });

    void DotNetRun(){
        DotNetTasks.DotNetRun(s => s
            .SetProjectFile(Solution.GetProject("PantryPad"))
        );
    }

    void NpmStart(){
        var npmStart = NpmTasks.NpmRun(s => s
            .SetProcessWorkingDirectory(RootDirectory / "PantryPad" / "wwwroot" .ToString())
            .SetArguments("start")
        );
    }
}
