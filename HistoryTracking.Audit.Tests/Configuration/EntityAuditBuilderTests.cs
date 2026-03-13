using HistoryTracking.Audit.Configuration;
using Xunit;

namespace HistoryTracking.Audit.Tests.Configuration;

public class EntityAuditBuilderTests
{
    private class SampleEntity
    {
        public int IntProp { get; set; }
        public string StringProp { get; set; }
    }

    [Fact]
    public void HasName_SetsOverrideName()
    {
        var builder = new EntityAuditBuilder<SampleEntity>();
        builder.HasName("CustomName");
        Assert.Equal("CustomName", builder.OverrideName);
    }

    [Fact]
    public void Ignore_SetsIsIgnored()
    {
        var builder = new EntityAuditBuilder<SampleEntity>();
        builder.Ignore();
        Assert.True(builder.IsIgnored);
    }

    [Fact]
    public void Property_ValueType_ExtractsCorrectName()
    {
        var builder = new EntityAuditBuilder<SampleEntity>();
        // int is a value type — expression body is wrapped in a UnaryExpression (Convert)
        builder.Property(x => x.IntProp);
        Assert.True(builder.PropertyBuilders.ContainsKey("IntProp"));
    }

    [Fact]
    public void Property_ReferenceType_ExtractsCorrectName()
    {
        var builder = new EntityAuditBuilder<SampleEntity>();
        builder.Property(x => x.StringProp!);
        Assert.True(builder.PropertyBuilders.ContainsKey("StringProp"));
    }

    [Fact]
    public void Property_CalledTwice_ReturnsSameBuilderInstance()
    {
        var builder = new EntityAuditBuilder<SampleEntity>();
        var first  = builder.Property(x => x.StringProp!);
        var second = builder.Property(x => x.StringProp!);
        Assert.Same(first, second);
    }
}
