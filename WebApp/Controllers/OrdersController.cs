using Microsoft.AspNetCore.Mvc;
using WebApp.Entities;
using WebApp.Mappers;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IRepository<OrderEntity> _repository;

    public OrdersController(IRepository<OrderEntity> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetAll()
    {
        var entities = await _repository.GetAll();
        var models = OrderMapper.ToModelList(entities);
        return Ok(models);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetById(long id)
    {
        var entity = await _repository.GetById(id);
        if (entity == null)
            return NotFound();

        var model = OrderMapper.ToModel(entity);
        return Ok(model);
    }

    [HttpGet("client/{clientId}")]
    public async Task<ActionResult<IEnumerable<Order>>> GetByClientId(long clientId)
    {
        var entities = await _repository.GetAll(o => o.ClientId == clientId);
        var models = OrderMapper.ToModelList(entities);
        return Ok(models);
    }

    [HttpPost]
    public async Task<ActionResult<Order>> Create(Order model)
    {
        var entity = OrderMapper.ToEntity(model);
        entity.Id = await _repository.Add(entity);

        var resultModel = OrderMapper.ToModel(entity);
        return CreatedAtAction(nameof(GetById), new { id = resultModel.Id }, resultModel);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, Order model)
    {
        if (id != model.Id)
            return BadRequest();

        var entity = OrderMapper.ToEntity(model);
        var updated = await _repository.Update(entity);
        if (updated == 0)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _repository.Delete(id);
        if (deleted == 0)
            return NotFound();

        return NoContent();
    }
}
