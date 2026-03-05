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

    public async Task<IAuditScope> CreateScopeAsync(AuditScopeDetails details)
    {
        var actionDefinitionId = await _repository
            .GetOrCreateActionDefinitionIdAsync(details.Code, details.Name);

        var parentScope = _scope;

        var actionLog = new ActionLogEntity
        {
            Timestamp          = DateTime.UtcNow,
            ActionDefinitionId = actionDefinitionId,
            ParentActionLogId  = parentScope?.ActionLogId
        };

        var actionLogId = await _repository.SaveActionLog(actionLog);
#pragma warning disable IDISP003 // parentScope is captured and restored on dispose; no scope is abandoned
        var scope = new AuditScope(actionLogId, () => _scope = parentScope);
        _scope = scope;
#pragma warning restore IDISP003
        return scope;
    }

    public async Task<long> GetOrCreateActionLogIdAsync()
    {
        if (_scope != null)
            return _scope.ActionLogId;

        var actionLog = new ActionLogEntity { Timestamp = DateTime.UtcNow };
        var actionLogId = await _repository.SaveActionLog(actionLog);
#pragma warning disable IDISP003 // false positive: _scope is null here, guarded by the early return above
        _scope ??= new AuditScope(actionLogId, static () => { });
#pragma warning restore IDISP003
        return actionLogId;
    }

    public void Dispose() => _scope?.Dispose();
}
