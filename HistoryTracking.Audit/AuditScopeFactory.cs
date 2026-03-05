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

        var actionLogId = await _repository.SaveActionLog(actionLog);
#pragma warning disable IDISP003
        var scope = new AuditScope(actionLogId, () =>
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

    public async Task<AuditScope> GetOrCreateActionLogIdAsync(long clientId)
    {
        if (_scopes.TryGetValue(clientId, out var existing))
            return existing;

        var actionLog = new ActionLogEntity { ClientId = clientId, Timestamp = DateTime.UtcNow };
        var actionLogId = await _repository.SaveActionLog(actionLog);
#pragma warning disable IDISP003
        var scope = new AuditScope(actionLogId, static () => { });
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
