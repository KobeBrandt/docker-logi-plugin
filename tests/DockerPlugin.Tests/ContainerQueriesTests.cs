namespace Loupedeck.DockerPlugin.Tests;

using Helpers;

using Types;

using Xunit;

public class ContainerQueriesTests
{
    private const String ProjectA = "project-a";
    private const String ProjectB = "project-b";

    [Fact]
    public void GetDisplayName_StripsNamePrefix() =>
        Assert.Equal("web", ContainerQueries.GetDisplayName(ContainerFactory.Running("web")));

    [Fact]
    public void GetDisplayName_NoNames_FallsBackToId() =>
        Assert.Equal("id1", ContainerQueries.GetDisplayName(new DockerContainer { Id = "id1" }));

    [Fact]
    public void IsRunning_DistinguishesStates()
    {
        Assert.True(ContainerQueries.IsRunning(ContainerFactory.Running("a")));
        Assert.False(ContainerQueries.IsRunning(ContainerFactory.Stopped("b")));
    }

    [Fact]
    public void HasDisplayName_MatchesNameWithoutPrefix() =>
        Assert.True(ContainerQueries.HasDisplayName("web")(ContainerFactory.Running("web")));

    [Fact]
    public void IsMostlyRunning_RequiresStrictMajority()
    {
        Assert.True(ContainerQueries.IsMostlyRunning([ContainerFactory.Running("a"), ContainerFactory.Running("b"), ContainerFactory.Stopped("c")]));
        Assert.False(ContainerQueries.IsMostlyRunning([ContainerFactory.Running("a"), ContainerFactory.Stopped("b")]));
    }

    [Fact]
    public void GetComposeProjects_ReturnsDistinctLabelledProjects()
    {
        var containers = new[]
        {
            ContainerFactory.Running("a", ProjectA),
            ContainerFactory.Stopped("b", ProjectA),
            ContainerFactory.Running("c", ProjectB),
            ContainerFactory.Running("unlabelled"),
        };

        Assert.Equal([ProjectA, ProjectB], ContainerQueries.GetComposeProjects(containers));
    }

    [Fact]
    public void FilterByComposeProject_ReturnsOnlyMatchingContainers()
    {
        var containers = new[] { ContainerFactory.Running("a", ProjectA), ContainerFactory.Running("b", ProjectB) };

        Assert.Equal("a", Assert.Single(ContainerQueries.FilterByComposeProject(containers, ProjectA)).Id);
    }
}
