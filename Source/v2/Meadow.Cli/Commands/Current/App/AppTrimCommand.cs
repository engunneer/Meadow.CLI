using CliFx.Attributes;
using Meadow.Software;
using Microsoft.Extensions.Logging;
using static Meadow.Software.FileManager;

namespace Meadow.CLI.Commands.DeviceManagement;

[Command("app trim", Description = "Runs an already-compiled Meadow application through reference trimming")]
public class AppTrimCommand : BaseAppCommand<AppTrimCommand>
{
    private FileManager _fileManager;

    [CommandOption('c', Description = "The build configuration to trim", IsRequired = false)]
    public string? Configuration { get; set; }

    [CommandParameter(0, Name = "Path to project file", IsRequired = false)]
    public string? Path { get; set; } = default!;

    public AppTrimCommand(
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
        string path = Path == null
            ? Environment.CurrentDirectory
            : Path;

        // is the path a file?
        FileInfo file;

        if (!File.Exists(path))
        {
            // is it a valid directory?
            if (!Directory.Exists(path))
            {
                Logger?.LogError($"Invalid application path '{path}'");
                return;
            }

            // it's a directory - we need to determine the latest build (they might have a Debug and a Release config)
            var candidates = PackageManager.GetAvailableBuiltConfigurations(path, false, "App.dll");

            if (candidates.Length == 0)
            {
                Logger?.LogError($"Cannot find a compiled application at '{path}'");
                return;
            }

            file = candidates.OrderByDescending(c => c.LastWriteTime).First();
        }
        else
        {
            file = new FileInfo(path);
        }

        string? targetRuntime;

        // Find RuntimeVersion
        if (Connection != null)
        {
            var info = await Connection.GetDeviceInfo(CancellationToken);

            targetRuntime = info?.RuntimeVersion;

            Logger?.LogInformation($"Using runtime files from {_packageManager.MeadowAssembliesPath}");

            // Avoid double reporting.
            DetachMessageHandlers(Connection);
        }
        else
        {
            // no device is attached - we need to choose a runtime version
            // for now we only support F7
            var store = _fileManager.Firmware[StoreNames.MeadowF7];
            await store.Refresh();

            targetRuntime = store!.DefaultPackage!.GetFullyQualifiedPath(store!.DefaultPackage!.Runtime ?? string.Empty);
        }

        if (targetRuntime == null)
        {
            Logger?.LogError("Cannot determine runtime to build against.");
        }
        else
        {
            // TODO: support a command line arg for runtime version
            // TODO: support `nolink` command line args

            var options = new BuildOptions(targetRuntime);

            await _packageManager.TrimApplication(file, options, Logger, CancellationToken)
                .WithSpinner(Console!, 250);
        }
    }
}