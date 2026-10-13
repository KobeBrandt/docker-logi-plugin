namespace Loupedeck.DockerPlugin.Helpers;

using System.IO.Pipes;
using System.Net.Sockets;

public static class DockerHttpClientFactory
{
    public static HttpClient CreateNamedPipeClient(String pipeName)
    {
        ArgumentNullException.ThrowIfNull(pipeName);
        var handler = new SocketsHttpHandler
        {
            ConnectCallback = (_, cancellationToken) => ConnectToNamedPipe(pipeName, cancellationToken),
        };
        return WithDockerDefaults(new HttpClient(handler), DockerConstants.LocalApiBaseUrl);
    }

    // The socket path is resolved per connection because Docker may start (and create its socket) after the plugin loads.
    public static HttpClient CreateUnixSocketClient(Func<String> socketPathProvider)
    {
        ArgumentNullException.ThrowIfNull(socketPathProvider);
        var handler = new SocketsHttpHandler
        {
            ConnectCallback = (_, cancellationToken) => ConnectToUnixSocket(socketPathProvider(), cancellationToken),
        };
        return WithDockerDefaults(new HttpClient(handler), DockerConstants.LocalApiBaseUrl);
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

    // ConnectAsync retries until the pipe exists, so a stopped Docker ends in cancellation, not an immediate error.
    private static async ValueTask<Stream> ConnectToNamedPipe(String pipeName, CancellationToken cancellationToken)
    {
        var pipe = new NamedPipeClientStream(DockerConstants.LocalPipeServer, pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
        try
        {
            await pipe.ConnectAsync(cancellationToken);
            return pipe;
        }
        catch
        {
            await pipe.DisposeAsync();
            throw;
        }
    }
}
