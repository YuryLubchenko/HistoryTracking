using HistoryTracking.Audit.Services;
using Microsoft.AspNetCore.Mvc;

namespace HistoryTracking.Audit.Controllers;

[ApiController]
[Route("api/audit")]
public class AuditController : ControllerBase
{
    private readonly IAuditQueryService _queryService;

    public AuditController(IAuditQueryService queryService)
    {
        _queryService = queryService;
    }

    [HttpGet("clients/{clientId}/action-logs/count")]
    public async Task<ActionResult<ActionLogCountResponse>> GetActionLogCount(long clientId)
    {
        var count = await _queryService.GetActionLogCountAsync(clientId);
        return Ok(new ActionLogCountResponse(count));
    }
}

public sealed record ActionLogCountResponse(long Count);
