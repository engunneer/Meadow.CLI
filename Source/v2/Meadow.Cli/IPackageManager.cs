using Microsoft.Extensions.Logging;

namespace Meadow.CLI;

public interface IPackageManager
{
    List<string> GetDependencies(FileInfo file, string runtimePath);

    bool BuildApplication(
        string projectFilePath,
        string configuration = "Release",
        bool clean = true,
        ILogger? logger = null,
        CancellationToken? cancellationToken = null);

    Task<IEnumerable<string>?> TrimApplication(
        FileInfo applicationFilePath,
        BuildOptions options,
        ILogger? logger = null,
        CancellationToken? cancellationToken = null);

    Task<string> AssemblePackage(
        string contentSourceFolder,
        string outputFolder,
        string osVersion,
        string filter = "*",
        bool overwrite = false,
        ILogger? logger = null,
        CancellationToken? cancellationToken = null);

    List<string>? AssemblyDependencies { get; set; }

    IEnumerable<string>? TrimmedDependencies { get; set; }

    string? RuntimeVersion { get; set; }
    string? MeadowAssembliesPath { get; }
}