using Meadow.Hcom;
using Meadow.Software;
using Microsoft.Extensions.Logging;
namespace Meadow.CLI;

public static class AppManager
{
    private static string[] dllLinkIngoreList = { "System.Threading.Tasks.Extensions.dll" };//, "Microsoft.Extensions.Primitives.dll" };
    private static string[] pdbLinkIngoreList = { "System.Threading.Tasks.Extensions.pdb" };//, "Microsoft.Extensions.Primitives.pdb" };

    public static string[] DoNotDeploy =
    {
        "System.Threading.Tasks.Extensions.dll",
        "System.Threading.Tasks.Extensions.pdb",
        ".DS_Store"
    };

    private static bool MatchingDllExists(string file)
    {
        var root = Path.GetFileNameWithoutExtension(file);
        return File.Exists($"{root}.dll");
    }

    private static bool IsPdb(string file)
    {
        return string.Compare(Path.GetExtension(file), ".pdb", true) == 0;
    }

    private static bool IsXmlDoc(string file)
    {
        if (string.Compare(Path.GetExtension(file), ".xml", true) == 0)
        {
            return MatchingDllExists(file);
        }
        return false;
    }

    public static Dictionary<string, uint> GenerateDeployList(
        string localBinaryDirectory,
        string? additionalFileDirectory,
        IEnumerable<string> dependencies,
        bool includePdbs)
    {
        var result = new Dictionary<string, uint>();

        var dependentFiles = dependencies.ToList();

        foreach (var file in Directory.GetFiles(localBinaryDirectory))
        {
            if (!includePdbs && Path.GetExtension(file) == ".pdb")
            {
                continue;
            }
            if (DoNotDeploy.Contains(Path.GetFileName(file)))
            {
                continue;
            }

            if (!result.ContainsKey(file))
            {
                var crc = GetFileCrc(file);
                result.Add(file, crc);
            }

            // if the file exists in the dependencies, remove it
            // (i.e. binary directory overrides dependency directory)
            var dep = dependentFiles.FirstOrDefault(f => Path.GetFileName(f) == Path.GetFileName(file));
            if (dep != null)
            {
                dependentFiles.Remove(dep);
            }
        }

        foreach (var dependentFile in dependentFiles)
        {
            var crc = GetFileCrc(dependentFile);
            result.Add(dependentFile, crc);
        }

        if (additionalFileDirectory != null && additionalFileDirectory != localBinaryDirectory)
        {
            // add any files that are in the "additional file directory" but not in the list we've built so far
            // this allows the addition of things like configs, data, etc. that are not in the dependency tree
            foreach (var file in Directory.GetFiles(additionalFileDirectory))
            {
                if (DoNotDeploy.Contains(Path.GetFileName(file)))
                {
                    continue;
                }

                // do not add DLL, EXE, PDB or doc XML
                switch (Path.GetExtension(file))
                {
                    case ".dll":
                    case ".exe":
                    case ".pdb":
                        continue;
                    case ".xml":
                        // if there's a matching DLL, skip
                        var root = Path.GetFileNameWithoutExtension(file);
                        var dll = result.Keys.FirstOrDefault(f => Path.GetFileName(f) == $"{root}.dll");
                        if (dll != null)
                        {
                            continue;
                        }
                        break;
                }

                var existing = result.Keys.FirstOrDefault(f => Path.GetFileName(f) == Path.GetFileName(file));
                if (existing == null)
                {
                    var crc = GetFileCrc(file);
                    result.Add(file, crc);
                }
            }
        }

        return result;
    }

    public static async Task<Dictionary<string, uint>> GenerateDeployList_old(
        IPackageManager packageManager,
        string localBinaryDirectory,
        bool includePdbs,
        ILogger? logger,
        CancellationToken cancellationToken)
    {
        // TODO: add sub-folder support when HCOM supports it

        logger?.LogInformation($"Generating the list of files to deploy from {localBinaryDirectory}...");

        var localFiles = new Dictionary<string, uint>();

        var auxiliary = Directory.EnumerateFiles(localBinaryDirectory, "*.*", SearchOption.TopDirectoryOnly)
                                       .Where(s => new FileInfo(s).Extension != ".dll")
                                       .Where(s => new FileInfo(s).Extension != ".pdb")
                                       .Where(s => !s.Contains(".DS_Store"));

        foreach (var item in auxiliary)
        {
            var file = Path.Combine(localBinaryDirectory, item);
            if (File.Exists(file))
            {
                await AddToLocalFiles(localFiles, file, includePdbs, cancellationToken);
            }
        }

        if (packageManager.TrimmedDependencies != null)
        {
            var trimmedDependencies = packageManager.TrimmedDependencies
                        .Where(x => dllLinkIngoreList.Any(f => x.Contains(f)) == false)
                        .Where(x => pdbLinkIngoreList.Any(f => x.Contains(f)) == false)
                        .ToList();

            // Crawl trimmed dependencies
            foreach (var file in trimmedDependencies)
            {
                await AddToLocalFiles(localFiles, file, includePdbs, cancellationToken);
            }

            // Add the Dlls from the TrimmingIgnorelist
            for (int i = 0; i < dllLinkIngoreList.Length; i++)
            {
                //add the files from the dll link ignore list
                if (packageManager.AssemblyDependencies!.Exists(f => f.Contains(dllLinkIngoreList[i])))
                {
                    var dllfound = packageManager.AssemblyDependencies!.FirstOrDefault(f => f.Contains(dllLinkIngoreList[i]));
                    if (!string.IsNullOrEmpty(dllfound))
                    {
                        await AddToLocalFiles(localFiles, dllfound, includePdbs, cancellationToken);
                    }
                }
            }

            if (includePdbs)
            {
                for (int i = 0; i < pdbLinkIngoreList.Length; i++)
                {
                    //add the files from the pdb link ignore list
                    if (packageManager.AssemblyDependencies!.Exists(f => f.Contains(pdbLinkIngoreList[i])))
                    {
                        var pdbFound = packageManager.AssemblyDependencies!.FirstOrDefault(f => f.Contains(pdbLinkIngoreList[i]));
                        if (!string.IsNullOrEmpty(pdbFound))
                        {
                            await AddToLocalFiles(localFiles, pdbFound, includePdbs, cancellationToken);
                        }
                    }
                }
            }
        }
        else
        {
            foreach (var file in packageManager.AssemblyDependencies!)
            {
                // TODO: add any other filtering capability here

                //Populate out LocalFile Dictionary with this entry
                await AddToLocalFiles(localFiles, file, includePdbs, cancellationToken);
            }
        }

        if (localFiles.Count() == 0)
        {
            logger?.LogInformation($"No new files to deploy");
        }

        logger?.LogInformation("Done.");

        return localFiles;
    }

    public static async Task DeployFilesToDevice(
        IMeadowConnection connection,
        Dictionary<string, uint> localFiles,
        ILogger? logger,
        CancellationToken cancellationToken)
    {
        // get a list of files on-device, with CRCs
        var deviceFiles = await connection.GetFileList(true, cancellationToken) ?? Array.Empty<MeadowFileInfo>();

        // get a list of files of the device files that are not in the list we intend to deploy
        var removeFiles = deviceFiles
            .Select(f => Path.GetFileName(f.Name))
            .Except(localFiles.Keys
                .Select(f => Path.GetFileName(f)));

        if (removeFiles.Count() == 0)
        {
            logger?.LogInformation($"No files to delete");
        }

        // delete those files
        foreach (var file in removeFiles)
        {
            logger?.LogInformation($"Deleting file '{file}'...");
            await connection.DeleteFile(file, cancellationToken);
        }

        // now send all files with differing CRCs
        foreach (var localFile in localFiles)
        {
            if (!File.Exists(localFile.Key))
            {
                logger?.LogInformation($"{localFile.Key} not found {Environment.NewLine}");
                continue;
            }

            var filename = Path.GetFileName(localFile.Key);

            var existing = deviceFiles.FirstOrDefault(f => Path.GetFileName(f.Name) == filename);

            if (existing != null && existing.Crc != null)
            {
                var remoteCrc = uint.Parse(existing.Crc.Substring(2), System.Globalization.NumberStyles.HexNumber);
                var localCrc = localFile.Value;

                // do the file name and CRC match?
                if (remoteCrc == localCrc)
                {
                    // exists and has a matching CRC, skip it
                    logger?.LogInformation($"Skipping file (hash match): {filename}{Environment.NewLine}");
                    continue;
                }
                else
                {
                    logger?.LogDebug($"{filename} 0x{localCrc:x8} != 0x{remoteCrc:x8}{Environment.NewLine}");
                }
            }

            bool success;

            do
            {
                try
                {
                    if (!await connection.WriteFile(localFile.Key, null, cancellationToken))
                    {
                        logger?.LogWarning($"Error sending '{Path.GetFileName(localFile.Key)}'.  Retrying.");
                        await Task.Delay(100);
                        success = false;
                    }
                    else
                    {
                        success = true;
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogWarning($"Error sending '{Path.GetFileName(localFile.Key)}' ({ex.Message}).  Retrying.");
                    await Task.Delay(100);
                    success = false;
                }

            } while (!success);
        }
    }

    private static uint GetFileCrc(string file)
    {
        using FileStream fs = File.Open(file, FileMode.Open);
        var len = (int)fs.Length;
        var bytes = new byte[len];
        fs.Read(bytes, 0, len);

        return CrcTools.Crc32part(bytes, len, 0);
    }

    private static async Task AddToLocalFiles(Dictionary<string, uint> localFiles, string file, bool includePdbs, CancellationToken cancellationToken)
    {
        if (!includePdbs && IsPdb(file))
        {
            return;
        }
        if (IsXmlDoc(file))
        {
            return;
        }

        // read the file data so we can generate a CRC
        using FileStream fs = File.Open(file, FileMode.Open);
        var len = (int)fs.Length;
        var bytes = new byte[len];

        await fs.ReadAsync(bytes, 0, len, cancellationToken);

        var crc = CrcTools.Crc32part(bytes, len, 0);

        if (!localFiles.ContainsKey(file))
        {
            localFiles.Add(file, crc);
        }
    }
}