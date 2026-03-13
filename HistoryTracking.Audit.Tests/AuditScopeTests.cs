using Xunit;

namespace HistoryTracking.Audit.Tests;

public class AuditScopeTests
{
    [Fact]
    public void ActionLogId_IsStoredCorrectly()
    {
        var scope = new AuditScope(42L, () => { });
        Assert.Equal(42L, scope.ActionLogId);
    }

    [Fact]
    public void Dispose_InvokesCallback()
    {
        var called = 0;
        var scope = new AuditScope(1L, () => called++);

        scope.Dispose();

        Assert.Equal(1, called);
    }

    [Fact]
    public void Dispose_CalledTwice_CallbackOnlyOnce()
    {
        var called = 0;
        var scope = new AuditScope(1L, () => called++);

        scope.Dispose();
        scope.Dispose();

        Assert.Equal(1, called);
    }
}
