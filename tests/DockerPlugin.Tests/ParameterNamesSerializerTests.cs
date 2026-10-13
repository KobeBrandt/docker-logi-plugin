namespace Loupedeck.DockerPlugin.Tests;

using Helpers;

using Xunit;

public class ParameterNamesSerializerTests
{
    [Fact]
    public void Serialize_ThenDeserialize_ReturnsSameNames()
    {
        var json = ParameterNamesSerializer.Serialize(["a", "b"]);

        Assert.True(ParameterNamesSerializer.TryDeserialize(json, out var names));
        Assert.Equal(["a", "b"], names);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not json")]
    [InlineData("null")]
    public void TryDeserialize_MissingOrInvalidJson_ReturnsFalse(String json)
    {
        Assert.False(ParameterNamesSerializer.TryDeserialize(json, out var names));
        Assert.Null(names);
    }
}
