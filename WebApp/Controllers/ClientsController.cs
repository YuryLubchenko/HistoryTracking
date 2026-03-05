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
    private readonly IRepository<ContactEntity> _contactRepository;
    private readonly IAuditWriterService _auditWriter;

    public ClientsController(
        IRepository<ClientEntity> repository,
        IRepository<ContactEntity> contactRepository,
        IAuditWriterService auditWriter)
    {
        _repository        = repository;
        _contactRepository = contactRepository;
        _auditWriter       = auditWriter;
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
        var nextId = await _repository.NextIdAsync();

        using var scope = await _auditWriter.CreateScopeAsync(
            nextId, new AuditScopeDetails { ActionTypeId = ActionCodes.CreateClient });

        var entity = ClientMapper.ToEntity(model);
        entity.Id = nextId;
        await _repository.Add(entity);

        using var otherScope = _auditWriter.CreateScopeAsync(nextId, new AuditScopeDetails
        {
            ActionTypeId = ActionCodes.CustomAction,
        });

        var resultModel = ClientMapper.ToModel(entity);
        return CreatedAtAction(nameof(GetById), new { id = resultModel.Id }, resultModel);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, Client model)
    {
        if (id != model.Id)
            return BadRequest();

        using var scope = await _auditWriter.CreateScopeAsync(
            id,
            new AuditScopeDetails { ActionTypeId = ActionCodes.UpdateClient });

        var entity = ClientMapper.ToEntity(model);
        var updated = await _repository.Update(entity);
        if (updated == 0)
            return NotFound();

        return NoContent();
    }

    [HttpPost("with-contacts")]
    public async Task<ActionResult<Client>> CreateWithContacts(CreateClientRequest request)
    {
        var nextId = await _repository.NextIdAsync();

        using var scope = await _auditWriter.CreateScopeAsync(
            nextId, new AuditScopeDetails { ActionTypeId = ActionCodes.CreateClient });

        var clientEntity = new ClientEntity { Id = nextId, Name = request.Name };
        await _repository.Add(clientEntity);

        foreach (var contact in request.Contacts)
        {
            var contactEntity = ContactMapper.ToEntity(contact, clientEntity.Id);
            await _contactRepository.Add(contactEntity);
        }

        var resultModel = ClientMapper.ToModel(clientEntity);
        return CreatedAtAction(nameof(GetById), new { id = resultModel.Id }, resultModel);
    }

    [HttpPatch("disable")]
    public async Task<IActionResult> DisableMany(DisableClientsRequest request)
    {
        foreach (var id in request.Ids)
        {
            var entity = await _repository.GetById(id);
            if (entity == null)
                continue;

            entity.Disabled = true;

            using var scope = await _auditWriter.CreateScopeAsync(
                id, new AuditScopeDetails { ActionTypeId = ActionCodes.DisableClient });

            await _repository.Update(entity);
        }

        return NoContent();
    }

    [HttpPatch("disable2")]
    public async Task<IActionResult> DisableMany2(DisableClientsRequest request)
    {
        foreach (var id in request.Ids)
        {
            var entity = await _repository.GetById(id);
            if (entity == null)
                continue;

            entity.Disabled = true;

            await _repository.Update(entity);
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        using var scope = await _auditWriter.CreateScopeAsync(
            id,
            new AuditScopeDetails { ActionTypeId = ActionCodes.DeleteClient });

        var deleted = await _repository.Delete(id);
        if (deleted == 0)
            return NotFound();

        return NoContent();
    }
}
