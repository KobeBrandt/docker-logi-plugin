namespace Loupedeck.DockerPlugin.Tests.Fakes;

using System.Net;

public sealed class FakeHttpMessageHandler : HttpMessageHandler
{
    private const String TestBaseUrl = "http://docker.test/";

    private readonly Func<HttpRequestMessage, HttpResponseMessage> _respond;

    public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) => this._respond = respond;

    public List<HttpRequestMessage> Requests { get; } = new();

    public static FakeHttpMessageHandler Returning(HttpStatusCode statusCode, String body = "") =>
        new(_ => new HttpResponseMessage(statusCode) { Content = new StringContent(body) });

    public static FakeHttpMessageHandler Throwing(Exception exception) => new(_ => throw exception);

    public HttpClient CreateClient() => new(this) { BaseAddress = new Uri(TestBaseUrl) };

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        this.Requests.Add(request);
        return Task.FromResult(this._respond(request));
    }
}
