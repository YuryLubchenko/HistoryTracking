using HistoryTracking.Audit.Entities;
using HistoryTracking.Audit.Repositories;

namespace HistoryTracking.Audit;

internal class AuditScopeFactory : IAuditScopeFactory
{
    private static readonly AsyncLocal<AuditScope> CurrentScope = new();

    private readonly IAuditLogRepository _repository;
    private AuditScope _anonymousScope;

    public AuditScopeFactory(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<long> GetOrCreateActionLogIdAsync()
    {
        var current = CurrentScope.Value ?? _anonymousScope;
        if (current != null)
            return current.ActionLogId;

        var actionLog = new ActionLogEntity { Timestamp = DateTime.UtcNow };
        var actionLogId = await _repository.SaveActionLog(actionLog);
        _anonymousScope = new AuditScope(actionLogId, static () => { });
        return actionLogId;
    }

    public async Task<IAuditScope> CreateScopeAsync(AuditScopeDetails details)
    {
        var actionDefinitionId = await _repository
            .GetOrCreateActionDefinitionIdAsync(details.Code, details.Name);

        var parentContextScope = CurrentScope.Value;
        long? parentActionLogId = (parentContextScope ?? _anonymousScope)?.ActionLogId;

        var actionLog = new ActionLogEntity
        {
            Timestamp          = DateTime.UtcNow,
            ActionDefinitionId = actionDefinitionId,
            ParentActionLogId  = parentActionLogId
        };

        var actionLogId = await _repository.SaveActionLog(actionLog);
        var scope = new AuditScope(actionLogId, () => CurrentScope.Value = parentContextScope);
        CurrentScope.Value = scope;
        return scope;
    }
}
