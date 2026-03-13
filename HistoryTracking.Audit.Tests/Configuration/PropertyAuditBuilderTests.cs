using HistoryTracking.Audit.Configuration;
using Xunit;

namespace HistoryTracking.Audit.Tests.Configuration;

public class PropertyAuditBuilderTests
{
    [Fact]
    public void HasName_SetsOverrideName()
    {
        var builder = new PropertyAuditBuilder();
        builder.HasName("Renamed");
        Assert.Equal("Renamed", builder.OverrideName);
    }

    [Fact]
    public void Ignore_SetsIsIgnored()
    {
        var builder = new PropertyAuditBuilder();
        builder.Ignore();
        Assert.True(builder.IsIgnored);
    }

    [Fact]
    public void AlwaysLog_SetsIsAlwaysLogged()
    {
        var builder = new PropertyAuditBuilder();
        builder.AlwaysLog();
        Assert.True(builder.IsAlwaysLogged);
    }

    [Fact]
    public void FluentMethods_ReturnSameInstance()
    {
        var builder = new PropertyAuditBuilder();
        Assert.Same(builder, builder.HasName("X"));
        Assert.Same(builder, builder.Ignore());
        Assert.Same(builder, builder.AlwaysLog());
    }
}
