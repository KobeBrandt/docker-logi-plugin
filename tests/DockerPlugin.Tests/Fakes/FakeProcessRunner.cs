namespace Loupedeck.DockerPlugin.Tests.Fakes;

using Helpers;

public sealed class FakeProcessRunner : IProcessRunner
{
    private readonly Func<Int32> _run;

    private FakeProcessRunner(Func<Int32> run) => this._run = run;

    public static FakeProcessRunner ExitingWith(Int32 exitCode) => new(() => exitCode);

    public static FakeProcessRunner Throwing(Exception exception) => new(() => throw exception);

    public String LastFileName { get; private set; }

    public Int32 RunAndGetExitCode(String fileName, String arguments)
    {
        this.LastFileName = fileName;
        return this._run();
    }
}
