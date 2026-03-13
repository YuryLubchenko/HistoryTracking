using HistoryTracking.Audit.Configuration;
using HistoryTracking.Audit.Entities;
using HistoryTracking.Audit.Repositories;
using HistoryTracking.Audit.Services;
using LinqToDB.Mapping;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using NSubstitute;
using Xunit;
using ServiceActionType = HistoryTracking.Audit.Services.ActionType;

namespace HistoryTracking.Audit.Tests;

public class AuditWriterServiceTests
{
    [Table("writer_test")]
    private class TestEntity
    {
        [PrimaryKey]
        [Column("id")]
        public long Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("secret")]
        public string Secret { get; set; }
    }

    private const long ClientId = 1L;
    private const string ToggleName = "audit-toggle";

    private readonly IAuditScopeFactory       _scopeFactory;
    private readonly IAuditLogRepository      _repo;
    private readonly IFeatureManager          _featureManager;
    private readonly IOptions<AuditOptions>   _options;

    public AuditWriterServiceTests()
    {
        _scopeFactory   = Substitute.For<IAuditScopeFactory>();
        _repo           = Substitute.For<IAuditLogRepository>();
        _featureManager = Substitute.For<IFeatureManager>();
        _options        = Substitute.For<IOptions<AuditOptions>>();
    }

    private AuditWriterService BuildService(
        AuditModel model = null,
        string toggleName = ToggleName)
    {
        _options.Value.Returns(new AuditOptions { FeatureToggleName = toggleName });
        return new AuditWriterService(
            _scopeFactory,
            _repo,
            model ?? new AuditModel(new Dictionary<Type, AuditEntityConfig>(), "test"),
            _options,
            _featureManager);
    }

    private void SetToggle(bool enabled)
        => _featureManager.IsEnabledAsync(ToggleName).Returns(Task.FromResult(enabled));

    // ── CreateScopeAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task CreateScopeAsync_ToggleDisabled_ReturnsNullAuditScope()
    {
        SetToggle(false);
        var service = BuildService();

        var scope = await service.CreateScopeAsync(ClientId, new AuditScopeDetails());

        Assert.Same(NullAuditScope.Instance, scope);
        await _scopeFactory.DidNotReceive().CreateScopeAsync(Arg.Any<long>(), Arg.Any<AuditScopeDetails>());
    }

    [Fact]
    public async Task CreateScopeAsync_NoToggleName_ReturnsNullAuditScope()
    {
        SetToggle(false);
        var service = BuildService(toggleName: null);

        var scope = await service.CreateScopeAsync(ClientId, new AuditScopeDetails());

        Assert.Same(NullAuditScope.Instance, scope);
        await _scopeFactory.DidNotReceive().CreateScopeAsync(Arg.Any<long>(), Arg.Any<AuditScopeDetails>());
    }

    [Fact]
    public async Task CreateScopeAsync_ToggleEnabled_DelegatesToFactory()
    {
        SetToggle(true);
        var fakeScope = new AuditScope(8L, () => { });
        _scopeFactory.CreateScopeAsync(ClientId, Arg.Any<AuditScopeDetails>())
                     .Returns(Task.FromResult(fakeScope));

        var scope = await BuildService().CreateScopeAsync(ClientId, new AuditScopeDetails());

        Assert.Same(fakeScope, scope);
    }

    // ── HandleEntityChangedAsync ──────────────────────────────────────────────

    [Fact]
    public async Task HandleEntityChangedAsync_ToggleDisabled_DoesNotCallRepository()
    {
        SetToggle(false);

        await BuildService().HandleEntityChangedAsync(ClientId,
            new TestEntity { Id = 1, Name = "Old" },
            new TestEntity { Id = 1, Name = "New" },
            ServiceActionType.Updated);

        await _repo.DidNotReceive().SaveEntityChange(Arg.Any<EntityRecordEntity>());
    }

    [Fact]
    public async Task HandleEntityChangedAsync_IgnoredEntity_DoesNotSaveEntityRecord()
    {
        SetToggle(true);
        var entityConfig = new AuditEntityConfig { IsIgnored = true };
        var model = new AuditModel(
            new Dictionary<Type, AuditEntityConfig> { [typeof(TestEntity)] = entityConfig },
            "test");

        await BuildService(model).HandleEntityChangedAsync(ClientId,
            new TestEntity { Id = 1, Name = "X" },
            new TestEntity { Id = 1, Name = "Y" },
            ServiceActionType.Updated);

        await _repo.DidNotReceive().SaveEntityChange(Arg.Any<EntityRecordEntity>());
    }

    [Fact]
    public async Task HandleEntityChangedAsync_Created_SavesEntityRecord()
    {
        SetToggle(true);
        var currentScope = new AuditScope(99L, () => { });
        _scopeFactory.GetOrCreateActionScopeAsync(ClientId).Returns(Task.FromResult(currentScope));
        _repo.GetOrCreateEntityTypeIdAsync("WriterTest").Returns(Task.FromResult(5L));
        _repo.SaveEntityChange(Arg.Any<EntityRecordEntity>()).Returns(Task.FromResult(10L));
        _repo.GetOrCreatePropertyDefinitionIdAsync(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<string>())
             .Returns(Task.FromResult(20L));

        await BuildService().HandleEntityChangedAsync(ClientId,
            null!,
            new TestEntity { Id = 1, Name = "Alice" },
            ServiceActionType.Created);

        await _repo.Received(1).SaveEntityChange(Arg.Is<EntityRecordEntity>(e =>
            e.ActionLogId == 99L &&
            e.ActionType == Entities.ActionType.Created));
    }

    [Fact]
    public async Task HandleEntityChangedAsync_WithPropertyChanges_SavesPropertyRecords()
    {
        SetToggle(true);
        var currentScope = new AuditScope(99L, () => { });
        _scopeFactory.GetOrCreateActionScopeAsync(ClientId).Returns(Task.FromResult(currentScope));
        _repo.GetOrCreateEntityTypeIdAsync("WriterTest").Returns(Task.FromResult(5L));
        _repo.SaveEntityChange(Arg.Any<EntityRecordEntity>()).Returns(Task.FromResult(10L));
        _repo.GetOrCreatePropertyDefinitionIdAsync(5L, "Name", "System.String")
             .Returns(Task.FromResult(20L));

        await BuildService().HandleEntityChangedAsync(ClientId,
            new TestEntity { Id = 1, Name = "Old" },
            new TestEntity { Id = 1, Name = "New" },
            ServiceActionType.Updated);

        await _repo.Received(1).SavePropertyChanges(Arg.Is<IEnumerable<PropertyRecordEntity>>(records =>
            records.Any(p => p.OldValue == "Old" && p.NewValue == "New")));
    }

    [Fact]
    public async Task HandleEntityChangedAsync_IgnoredProperty_SkippedInSave()
    {
        SetToggle(true);
        var entityConfig = new AuditEntityConfig
        {
            Properties = new Dictionary<string, AuditPropertyConfig>
            {
                ["Secret"] = new AuditPropertyConfig { IsIgnored = true }
            }
        };
        var model = new AuditModel(
            new Dictionary<Type, AuditEntityConfig> { [typeof(TestEntity)] = entityConfig },
            "test");

        var currentScope = new AuditScope(99L, () => { });
        _scopeFactory.GetOrCreateActionScopeAsync(ClientId).Returns(Task.FromResult(currentScope));
        _repo.GetOrCreateEntityTypeIdAsync(Arg.Any<string>()).Returns(Task.FromResult(5L));
        _repo.SaveEntityChange(Arg.Any<EntityRecordEntity>()).Returns(Task.FromResult(10L));

        // old and new differ only in Secret (which is ignored) — no property records should be saved
        await BuildService(model).HandleEntityChangedAsync(ClientId,
            new TestEntity { Id = 1, Secret = "hidden-old" },
            new TestEntity { Id = 1, Secret = "hidden-new" },
            ServiceActionType.Updated);

        await _repo.DidNotReceive().SavePropertyChanges(Arg.Any<IEnumerable<PropertyRecordEntity>>());
    }
}
