using HistoryTracking.Audit;
using HistoryTracking.Audit.Services;
using Microsoft.AspNetCore.Mvc;
using WebApp.Entities;
using WebApp.Mappers;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers;

[ApiController]
[Route("api/clients/{clientId}/contacts")]
public class ContactsController : ControllerBase
{
    private readonly IRepository<ContactEntity> _repository;
    private readonly IAuditWriterService _auditWriter;

    public ContactsController(IRepository<ContactEntity> repository, IAuditWriterService auditWriter)
    {
        _repository  = repository;
        _auditWriter = auditWriter;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Contact>>> GetAll(long clientId)
    {
        var entities = await _repository.GetAll(c => c.ClientId == clientId);
        var models = ContactMapper.ToModelList(entities);
        return Ok(models);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Contact>> GetById(long clientId, long id)
    {
        var entity = await _repository.GetById(id);
        if (entity == null || entity.ClientId != clientId)
            return NotFound();

        var model = ContactMapper.ToModel(entity);
        return Ok(model);
    }

    [HttpPost]
    public async Task<ActionResult<Contact>> Create(long clientId, Contact model)
    {
        using var scope = await _auditWriter.CreateScopeAsync(
            clientId,
            new AuditScopeDetails { ActionTypeId = ActionCodes.CreateContact });

        var entity = ContactMapper.ToEntity(model, clientId);
        entity.Id = await _repository.Add(entity);

        var resultModel = ContactMapper.ToModel(entity);
        return CreatedAtAction(nameof(GetById), new { clientId, id = resultModel.Id }, resultModel);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long clientId, long id, Contact model)
    {
        if (id != model.Id)
            return BadRequest();

        var existing = await _repository.GetById(id);
        if (existing == null || existing.ClientId != clientId)
            return NotFound();

        using var scope = await _auditWriter.CreateScopeAsync(
            clientId,
            new AuditScopeDetails { ActionTypeId = ActionCodes.UpdateContact });

        var entity = ContactMapper.ToEntity(model, clientId);
        var updated = await _repository.Update(entity);
        if (updated == 0)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long clientId, long id)
    {
        var existing = await _repository.GetById(id);
        if (existing == null || existing.ClientId != clientId)
            return NotFound();

        using var scope = await _auditWriter.CreateScopeAsync(
            clientId,
            new AuditScopeDetails { ActionTypeId = ActionCodes.DeleteContact });

        var deleted = await _repository.Delete(id);
        if (deleted == 0)
            return NotFound();

        return NoContent();
    }
}
