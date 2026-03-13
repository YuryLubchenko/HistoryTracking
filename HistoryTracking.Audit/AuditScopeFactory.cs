using System.Collections.Concurrent;
using HistoryTracking.Audit.Entities;
using HistoryTracking.Audit.Repositories;

namespace HistoryTracking.Audit;

internal sealed class AuditScopeFactory : IAuditScopeFactory, IDisposable
{
    private readonly IAuditLogRepository _repository;
    private readonly ConcurrentDictionary<long, AuditScope> _scopes = new();

    public AuditScopeFactory(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<AuditScope> CreateScopeAsync(long clientId, AuditScopeDetails details)
    {
        _scopes.TryGetValue(clientId, out var parentScope);

        var actionLog = new ActionLogEntity
        {
            ClientId          = clientId,
            Timestamp         = DateTime.UtcNow,
            ActionTypeId      = details.ActionTypeId,
            ParentActionLogId = parentScope?.ActionLogId
        };

        await _repository.SaveActionLog(actionLog);
#pragma warning disable IDISP003
        var scope = new AuditScope(actionLog.Id, () =>
        {
            if (parentScope != null)
                _scopes[clientId] = parentScope;
            else
                _scopes.TryRemove(clientId, out _);
        });
#pragma warning restore IDISP003
        _scopes[clientId] = scope;
        return scope;
    }

    public async Task<AuditScope> GetOrCreateActionScopeAsync(long clientId)
    {
        if (_scopes.TryGetValue(clientId, out var existing))
            return existing;

        var actionLog = new ActionLogEntity { ClientId = clientId, Timestamp = DateTime.UtcNow };
        await _repository.SaveActionLog(actionLog);
#pragma warning disable IDISP003
        var scope = new AuditScope(actionLog.Id, static () => { });
#pragma warning restore IDISP003
        _scopes[clientId] = scope;
        return scope;
    }

    public void Dispose()
    {
        foreach (var scope in _scopes.Values)
            scope.Dispose();
        _scopes.Clear();
    }
}
