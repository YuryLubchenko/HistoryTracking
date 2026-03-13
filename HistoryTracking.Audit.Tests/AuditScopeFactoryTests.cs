using HistoryTracking.Audit.Entities;
using HistoryTracking.Audit.Repositories;
using NSubstitute;
using Xunit;

namespace HistoryTracking.Audit.Tests;

public class AuditScopeFactoryTests
{
    private static AuditScopeFactory CreateFactory(IAuditLogRepository repo)
        => new AuditScopeFactory(repo);

    private static IAuditLogRepository RepoReturning(params long[] ids)
    {
        var sub = Substitute.For<IAuditLogRepository>();
        var callIndex = 0;
        sub.SaveActionLog(Arg.Any<ActionLogEntity>())
           .Returns(callInfo =>
           {
               if (callIndex < ids.Length)
                   callInfo.Arg<ActionLogEntity>().Id = ids[callIndex++];
               return Task.CompletedTask;
           });
        return sub;
    }

    [Fact]
    public async Task CreateScopeAsync_NoParent_ReturnsCorrectId()
    {
        var factory = CreateFactory(RepoReturning(77L));

        var scope = await factory.CreateScopeAsync(1L, new AuditScopeDetails());

        Assert.Equal(77L, scope.ActionLogId);
    }

    [Fact]
    public async Task CreateScopeAsync_NoParent_ParentActionLogIdIsNull()
    {
        var capturedLogs = new List<ActionLogEntity>();
        var sub = Substitute.For<IAuditLogRepository>();
        sub.SaveActionLog(Arg.Any<ActionLogEntity>())
           .Returns(callInfo =>
           {
               var e = callInfo.Arg<ActionLogEntity>();
               capturedLogs.Add(e);
               e.Id = 1L;
               return Task.CompletedTask;
           });

        await CreateFactory(sub).CreateScopeAsync(1L, new AuditScopeDetails());

        Assert.Null(capturedLogs.Single().ParentActionLogId);
    }

    [Fact]
    public async Task CreateScopeAsync_WithActiveScope_SetsParentActionLogId()
    {
        var capturedLogs = new List<ActionLogEntity>();
        var callCount = 0;
        var sub = Substitute.For<IAuditLogRepository>();
        sub.SaveActionLog(Arg.Any<ActionLogEntity>())
           .Returns(callInfo =>
           {
               var e = callInfo.Arg<ActionLogEntity>();
               capturedLogs.Add(e);
               e.Id = ++callCount == 1 ? 100L : 200L;
               return Task.CompletedTask;
           });

        var factory = CreateFactory(sub);
        await factory.CreateScopeAsync(1L, new AuditScopeDetails());
        await factory.CreateScopeAsync(1L, new AuditScopeDetails());

        Assert.Null(capturedLogs[0].ParentActionLogId);
        Assert.Equal(100L, capturedLogs[1].ParentActionLogId);
    }

    [Fact]
    public async Task CreateScopeAsync_AfterChildDisposed_RestoresParentScope()
    {
        var factory = CreateFactory(RepoReturning(100L, 200L));

        var parentScope = await factory.CreateScopeAsync(1L, new AuditScopeDetails());
        var childScope  = await factory.CreateScopeAsync(1L, new AuditScopeDetails());
        childScope.Dispose();

        // No new DB call — parent should be returned from _scopes
        var restored = await factory.GetOrCreateActionScopeAsync(1L);

        Assert.Equal(100L, restored.ActionLogId);
    }

    [Fact]
    public async Task GetOrCreateActionScopeAsync_ExistingScope_ReturnsSameInstance()
    {
        var sub = RepoReturning(55L);
        var factory = CreateFactory(sub);

        var first  = await factory.GetOrCreateActionScopeAsync(1L);
        var second = await factory.GetOrCreateActionScopeAsync(1L);

        Assert.Same(first, second);
        await sub.Received(1).SaveActionLog(Arg.Any<ActionLogEntity>());
    }

    [Fact]
    public async Task GetOrCreateActionScopeAsync_NoScope_CreatesNewActionLog()
    {
        var factory = CreateFactory(RepoReturning(33L));

        var scope = await factory.GetOrCreateActionScopeAsync(1L);

        Assert.Equal(33L, scope.ActionLogId);
    }

    [Fact]
    public async Task Dispose_DoesNotThrow()
    {
        var factory = CreateFactory(RepoReturning(1L));
        await factory.GetOrCreateActionScopeAsync(1L);

        var ex = Record.Exception(() => factory.Dispose());

        Assert.Null(ex);
    }
}
