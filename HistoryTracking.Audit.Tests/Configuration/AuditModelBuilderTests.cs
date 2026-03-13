using System.Reflection;
using HistoryTracking.Audit.Configuration;
using Xunit;

namespace HistoryTracking.Audit.Tests.Configuration;

// Stub entity + config used by the assembly-scan test below
internal class StubEntity { }

internal class StubEntityConfig : IAuditEntityConfiguration<StubEntity>
{
    public void Configure(IEntityAuditBuilder<StubEntity> builder)
        => builder.HasName("StubOverride");
}

public class AuditModelBuilderTests
{
    private class EntityA { }

    private class EntityB
    {
        public string SomeProp { get; set; }
    }

    private class EntityAConfig : IAuditEntityConfiguration<EntityA>
    {
        public void Configure(IEntityAuditBuilder<EntityA> builder)
            => builder.HasName("EntityAOverride").Ignore();
    }

    private class EntityBConfig : IAuditEntityConfiguration<EntityB>
    {
        public void Configure(IEntityAuditBuilder<EntityB> builder)
            => builder.Property(x => x.SomeProp!).HasName("PropOverride").Ignore().AlwaysLog();
    }

    [Fact]
    public void ApplyConfiguration_AddsEntityBuilder()
    {
        var builder = new AuditModelBuilder();
        builder.ApplyConfiguration(new EntityAConfig());

        var model = builder.Build("test");

        Assert.NotNull(model.GetEntityConfig(typeof(EntityA)));
    }

    [Fact]
    public void Build_CreatesModelWithCorrectSchemaName()
    {
        var model = new AuditModelBuilder().Build("hist_schema");
        Assert.Equal("hist_schema", model.SchemaName);
    }

    [Fact]
    public void Build_EntityConfig_IgnoreAndOverrideNamePreserved()
    {
        var builder = new AuditModelBuilder();
        builder.ApplyConfiguration(new EntityAConfig());

        var config = builder.Build("s").GetEntityConfig(typeof(EntityA));

        Assert.Equal("EntityAOverride", config.OverrideName);
        Assert.True(config.IsIgnored);
    }

    [Fact]
    public void Build_PropertyConfig_AllFlagsPreserved()
    {
        var builder = new AuditModelBuilder();
        builder.ApplyConfiguration(new EntityBConfig());

        var entityConfig = builder.Build("s").GetEntityConfig(typeof(EntityB));
        var propConfig = entityConfig.GetPropertyConfig("SomeProp");

        Assert.NotNull(propConfig);
        Assert.Equal("PropOverride", propConfig!.OverrideName);
        Assert.True(propConfig.IsIgnored);
        Assert.True(propConfig.IsAlwaysLoged);
    }

    [Fact]
    public void ApplyConfigurationsFromAssembly_DiscoversStubConfig()
    {
        var builder = new AuditModelBuilder();
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        var model  = builder.Build("test");
        var config = model.GetEntityConfig(typeof(StubEntity));

        Assert.NotNull(config);
        Assert.Equal("StubOverride", config.OverrideName);
    }
}
