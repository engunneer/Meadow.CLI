using CliFx.Attributes;
using Meadow.Software;
using Microsoft.Extensions.Logging;
using static Meadow.Software.FileManager;

namespace Meadow.CLI.Commands.DeviceManagement;

[Command("app deploy", Description = "Deploys a built Meadow application to a target device")]
public class AppDeployCommand : BaseAppCommand<AppDeployCommand>
{
    private string _lastFile = string.Empty;
    private FileManager _fileManager;

    [CommandParameter(0, Name = "Path to folder containing the built application", IsRequired = false)]
    public string? Path { get; set; } = default!;

    [CommandOption("no-trim", Description = "Skip linking/trimming during deploy", IsRequired = false)]
    public bool NoTrim { get; set; }

    public AppDeployCommand(
        IPackageManager packageManager,
        FileManager fileManager,
        MeadowConnectionManager connectionManager,
        ILoggerFactory loggerFactory)
        : base(packageManager, connectionManager, loggerFactory)
    {
        _fileManager = fileManager;
    }

    protected override async ValueTask ExecuteCommand()
    {
        await base.ExecuteCommand();

        if (Connection != null)
        {
            string path = Path == null
                ? Environment.CurrentDirectory
                : Path;

            // is the path a file?
            FileInfo file;

            _lastFile = string.Empty;

            // in order to deploy, the runtime must be disabled
            var wasRuntimeEnabled = await Connection.IsRuntimeEnabled();

            if (wasRuntimeEnabled)
            {
                Logger?.LogInformation("Disabling runtime...");

                await Connection.RuntimeDisable(CancellationToken);
            }

            if (!File.Exists(path))
            {
                // is it a valid directory?
                if (!Directory.Exists(path))
                {
                    Logger?.LogError($"Invalid application path '{path}'");
                    return;
                }

                // does the directory have an App.dll in it?
                file = new FileInfo(System.IO.Path.Combine(path, "App.dll"));
                if (!file.Exists)
                {
                    // it's a directory - we need to determine the latest build (they might have a Debug and a Release config)
                    // do *not* look for existing trimmed folders, however
                    var candidates = PackageManager.GetAvailableBuiltConfigurations(path, false, "App.dll");

                    if (candidates.Length == 0)
                    {
                        Logger?.LogError($"Cannot find a compiled application at '{path}'");
                        return;
                    }

                    file = candidates.OrderByDescending(c => c.LastWriteTime).First();
                }
            }
            else
            {
                // TODO: only deploy if it's App.dll
                file = new FileInfo(path);
            }

            if (file.DirectoryName == null)
            {
                Logger?.LogError($"Invalid target directory '{path}'");
                return;
            }

            var binarySourceDirectory = file.DirectoryName;
            var additionalFileDirectory = file.DirectoryName;

            var store = _fileManager.Firmware[StoreNames.MeadowF7];
            await store.Refresh();

            // determine the runtime on the device for dev=pendencies
            var info = await Connection.Device!.GetDeviceInfo(CancellationToken);

            if (info == null || info.RuntimeVersion == null)
            {
                Logger?.LogError($"Unable to query device's runtime version information");
                return;
            }

            var package = store.GetPackageForVersion(info.RuntimeVersion ?? string.Empty);

            if (package == null)
            {
                Logger?.LogError($"Device has runtime {info.RuntimeVersion} but no local match was found.  Try downloading it.");
                return;
            }

            var runtimeFolder = package.GetFullyQualifiedPath(package.BclFolder ?? string.Empty);

            if (NoTrim)
            {
                // does a postlink folder exist?
                var postlink = System.IO.Path.Combine(file.DirectoryName, PackageManager.PostLinkDirectoryName);

                if (Directory.Exists(postlink))
                {
                    binarySourceDirectory = postlink;

                    // dev note: do *not* change to the App in postlink - we want the dependency tree of the original
                    //file = new FileInfo(System.IO.Path.Combine(postlink, "App.dll"));
                }
            }
            else
            {
                var options = new BuildOptions(runtimeFolder);

                using (var spinner = ConsoleSpinner.Create(Console))
                {
                    await _packageManager.TrimApplication(file, options, Logger, CancellationToken);
                }

                // the output is in a different location, so get the newly built folder
                binarySourceDirectory = PackageManager
                    .GetAvailableBuiltConfigurations(path, true, "App.dll")
                    .OrderByDescending(c => c.LastWriteTime)
                    .First()
                    .DirectoryName ?? string.Empty;

                // dev note: do *not* change to the App in postlink - we want the dependency tree of the original
                //file = new FileInfo(System.IO.Path.Combine(postlink, "App.dll"));
            }

            Logger?.LogInformation("Creating a list of files to deploy...");
            Dictionary<string, uint> localFiles;

            using (var spinner = ConsoleSpinner.Create(Console))
            {
                // determine what files to deploy
                var dependencies = _packageManager.GetDependencies(file, runtimeFolder);

                // TODO: add support for args to control deploy PDBs (y/n)

                // TODO: add any "special" files to deploy - i.e. files in the build output that aren't in the 
                // dependency tree (like app configs, data files, etc)
                var buildOutputFolder = System.IO.Path.GetDirectoryName(binarySourceDirectory);

                localFiles = AppManager.GenerateDeployList(
                    binarySourceDirectory,
                    additionalFileDirectory,
                    dependencies ?? new List<string>(),
                    binarySourceDirectory.Contains("Debug"));

            }

            Connection.FileWriteProgress += Connection_FileWriteProgress;

            await AppManager.DeployFilesToDevice(Connection, localFiles, Logger, CancellationToken);
            Console?.Output.WriteAsync("\n");

            Connection.FileWriteProgress -= Connection_FileWriteProgress;


            if (wasRuntimeEnabled)
            {
                // restore runtime state
                Logger?.LogInformation("Enabling runtime...");

                await Connection.RuntimeEnable(CancellationToken);
            }
        }
    }

    private void Connection_FileWriteProgress(object? sender, (string fileName, long completed, long total) e)
    {
        var p = e.completed / (double)e.total * 100d;

        if (e.fileName != _lastFile)
        {
            Console?.Output.WriteAsync("\n");
            _lastFile = e.fileName;
        }

        // Console instead of Logger due to line breaking for progress bar
        Console?.Output.WriteAsync($"Writing {e.fileName}: {p:0}%         \r");
    }
}