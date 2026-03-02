using HistoryTracking.Audit.Entities;
using HistoryTracking.Audit.Repositories;

namespace HistoryTracking.Audit;

internal class HistoryContext : IAuditScopeFactory
{
    private readonly Stack<AuditScope> _scopes = new();
    private readonly IAuditLogRepository _repository;
    private long? _anonymousActionLogId;

    public HistoryContext(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<long> GetOrCreateActionLogIdAsync()
    {
        if (_scopes.Count > 0)
            return _scopes.Peek().ActionLogId;

        if (_anonymousActionLogId.HasValue)
            return _anonymousActionLogId.Value;

        var actionLog = new ActionLogEntity { Timestamp = DateTime.UtcNow };
        _anonymousActionLogId = await _repository.SaveActionLog(actionLog);
        return _anonymousActionLogId.Value;
    }

    public async Task<IAuditScope> CreateScopeAsync(AuditScopeDetails details)
    {
        var actionDefinitionId = await _repository
            .GetOrCreateActionDefinitionIdAsync(details.Code, details.Name);

        long? parentActionLogId = _scopes.Count > 0
            ? _scopes.Peek().ActionLogId
            : _anonymousActionLogId;

        var actionLog = new ActionLogEntity
        {
            Timestamp          = DateTime.UtcNow,
            ActionDefinitionId = actionDefinitionId,
            ParentActionLogId  = parentActionLogId
        };

        var actionLogId = await _repository.SaveActionLog(actionLog);
        var scope = new AuditScope(actionLogId, () => _scopes.TryPop(out _));
        _scopes.Push(scope);
        return scope;
    }
}
