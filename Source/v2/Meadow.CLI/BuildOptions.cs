namespace Meadow.CLI;

public record BuildOptions
{
    public BuildOptions(string runtimeFolder)
    {
        if (!Directory.Exists(runtimeFolder))
        {
            throw new ArgumentException("Provided runtime folder not found");
        }

        RuntimeFolder = runtimeFolder;
    }

    public DeployOptions Deploy { get; set; } = new();
    public string RuntimeFolder { get; set; }
    public bool VerboseOutput { get; set; } = false;
    public string? LinkerOptionsFile { get; set; } = null;

    public record DeployOptions
    {
        public List<string> NoLink { get; set; } = new();
        public bool IncludePDBs { get; set; } = false;
    }

    public void MergeWith(BuildOptions other)
    {
        if (Deploy == null)
        {
            Deploy = new DeployOptions();
        }

        if (other.Deploy?.NoLink != null && other.Deploy?.NoLink.Count > 0)
        {
            Deploy.NoLink = other.Deploy.NoLink;
        }

        if (other.Deploy?.IncludePDBs != null)
        {
            Deploy.IncludePDBs = other.Deploy.IncludePDBs;
        }

        if (other.RuntimeFolder != null && other.RuntimeFolder != string.Empty)
        {
            RuntimeFolder = other.RuntimeFolder;
        }
    }
}
