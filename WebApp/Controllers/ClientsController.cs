using HistoryTracking.Audit;
using HistoryTracking.Audit.Services;
using Microsoft.AspNetCore.Mvc;
using WebApp.Entities;
using WebApp.Mappers;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IRepository<ClientEntity> _repository;
    private readonly IAuditWriterService _auditWriter;

    public ClientsController(IRepository<ClientEntity> repository, IAuditWriterService auditWriter)
    {
        _repository   = repository;
        _auditWriter = auditWriter;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Client>>> GetAll()
    {
        var entities = await _repository.GetAll();
        var models = ClientMapper.ToModelList(entities);
        return Ok(models);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Client>> GetById(long id)
    {
        var entity = await _repository.GetById(id);
        if (entity == null)
            return NotFound();

        var model = ClientMapper.ToModel(entity);
        return Ok(model);
    }

    [HttpPost]
    public async Task<ActionResult<Client>> Create(Client model)
    {
        using var scope = await _auditWriter.CreateScopeAsync(
            new AuditScopeDetails { Code = "001", Name = "CreateClient" });

        var entity = ClientMapper.ToEntity(model);
        entity.Id = await _repository.Add(entity);

        var resultModel = ClientMapper.ToModel(entity);
        return CreatedAtAction(nameof(GetById), new { id = resultModel.Id }, resultModel);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, Client model)
    {
        if (id != model.Id)
            return BadRequest();

        using var scope = await _auditWriter.CreateScopeAsync(
            new AuditScopeDetails { Code = "002", Name = "UpdateClient" });

        var entity = ClientMapper.ToEntity(model);
        var updated = await _repository.Update(entity);
        if (updated == 0)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        using var scope = await _auditWriter.CreateScopeAsync(
            new AuditScopeDetails { Code = "003", Name = "DeleteClient" });

        var deleted = await _repository.Delete(id);
        if (deleted == 0)
            return NotFound();

        return NoContent();
    }
}
