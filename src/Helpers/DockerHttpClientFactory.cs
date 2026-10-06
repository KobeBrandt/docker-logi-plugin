namespace Loupedeck.DockerPlugin.Helpers;

using System.Net.Sockets;

public static class DockerHttpClientFactory
{
    public static HttpClient CreateTcpClient() => WithDockerDefaults(new HttpClient(), DockerConstants.ApiBaseUrl);

    // The socket path is resolved per connection because Docker may start (and create its socket) after the plugin loads.
    public static HttpClient CreateUnixSocketClient(Func<String> socketPathProvider)
    {
        ArgumentNullException.ThrowIfNull(socketPathProvider);
        var handler = new SocketsHttpHandler
        {
            ConnectCallback = (_, cancellationToken) => ConnectToUnixSocket(socketPathProvider(), cancellationToken),
        };
        return WithDockerDefaults(new HttpClient(handler), DockerConstants.UnixSocketApiBaseUrl);
    }

    private static HttpClient WithDockerDefaults(HttpClient httpClient, String baseUrl)
    {
        httpClient.BaseAddress = new Uri(baseUrl);
        httpClient.Timeout = DockerConstants.RequestTimeout;
        return httpClient;
    }

    private static async ValueTask<Stream> ConnectToUnixSocket(String socketPath, CancellationToken cancellationToken)
    {
        var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
        try
        {
            await socket.ConnectAsync(new UnixDomainSocketEndPoint(socketPath), cancellationToken);
            return new NetworkStream(socket, ownsSocket: true);
        }
        catch
        {
            socket.Dispose();
            throw;
        }
    }
}
