namespace Loupedeck.DockerPlugin.Helpers;

public sealed class DockerPathLocator
{
    private readonly Func<String, Boolean> _pathExists;
    private readonly String _homeDirectory;

    public DockerPathLocator(Func<String, Boolean> pathExists, String homeDirectory)
    {
        this._pathExists = pathExists ?? throw new ArgumentNullException(nameof(pathExists));
        this._homeDirectory = homeDirectory ?? throw new ArgumentNullException(nameof(homeDirectory));
    }

    public static DockerPathLocator CreateDefault() =>
        new(File.Exists, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

    public String FindSocketPath() =>
        this.FirstExisting(this.SocketCandidatePaths()) ?? DockerConstants.SystemSocketPath;

    // Apps launched from the macOS GUI do not inherit the shell PATH, so look in the usual install locations first.
    public String FindCliExecutable() =>
        this.FirstExisting(DockerConstants.UnixCliCandidatePaths) ?? DockerConstants.CliExecutable;

    private IEnumerable<String> SocketCandidatePaths() =>
        DockerConstants.UserSocketRelativePaths
            .Select(relativePath => Path.Combine(this._homeDirectory, relativePath))
            .Prepend(DockerConstants.SystemSocketPath);

    private String FirstExisting(IEnumerable<String> candidatePaths) => candidatePaths.FirstOrDefault(this._pathExists);
}
