using HistoryTracking.Audit.Entities;
using HistoryTracking.Audit.Repositories;

namespace HistoryTracking.Audit;

internal sealed class AuditScopeFactory : IAuditScopeFactory, IDisposable
{
    private readonly IAuditLogRepository _repository;

    private AuditScope _scope;

    public AuditScopeFactory(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<AuditScope> CreateScopeAsync(long clientId, AuditScopeDetails details)
    {
        var parentScope = _scope;

        var actionLog = new ActionLogEntity
        {
            ClientId           = clientId,
            Timestamp          = DateTime.UtcNow,
            ActionTypeId       = details.ActionTypeId,
            ParentActionLogId  = parentScope?.ActionLogId
        };

        var actionLogId = await _repository.SaveActionLog(actionLog);
#pragma warning disable IDISP003 // parentScope is captured and restored on dispose; no scope is abandoned
        var scope = new AuditScope(actionLogId, () => _scope = parentScope);
        _scope = scope;
#pragma warning restore IDISP003
        return scope;
    }

    public async Task<AuditScope> GetOrCreateActionLogIdAsync(long clientId)
    {
        if (_scope != null)
            return _scope;

        var actionLog = new ActionLogEntity { ClientId = clientId, Timestamp = DateTime.UtcNow };
        var actionLogId = await _repository.SaveActionLog(actionLog);
#pragma warning disable IDISP003 // false positive: _scope is null here, guarded by the early return above
        _scope ??= new AuditScope(actionLogId, static () => { });
#pragma warning restore IDISP003
        return _scope;
    }

    public void Dispose() => _scope?.Dispose();
}
