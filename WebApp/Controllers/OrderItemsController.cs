using Microsoft.AspNetCore.Mvc;
using WebApp.Entities;
using WebApp.Mappers;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderItemsController : ControllerBase
{
    private readonly IRepository<OrderItemEntity> _repository;

    public OrderItemsController(IRepository<OrderItemEntity> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderItem>>> GetAll()
    {
        var entities = await _repository.GetAll();
        var models = OrderItemMapper.ToModelList(entities);
        return Ok(models);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderItem>> GetById(long id)
    {
        var entity = await _repository.GetById(id);
        if (entity == null)
            return NotFound();

        var model = OrderItemMapper.ToModel(entity);
        return Ok(model);
    }

    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<IEnumerable<OrderItem>>> GetByOrderId(long orderId)
    {
        var entities = await _repository.GetAll(i => i.OrderId == orderId);
        var models = OrderItemMapper.ToModelList(entities);
        return Ok(models);
    }

    [HttpPost]
    public async Task<ActionResult<OrderItem>> Create(OrderItem model)
    {
        var entity = OrderItemMapper.ToEntity(model);
        entity.Id = await _repository.Add(entity);

        var resultModel = OrderItemMapper.ToModel(entity);
        return CreatedAtAction(nameof(GetById), new { id = resultModel.Id }, resultModel);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, OrderItem model)
    {
        if (id != model.Id)
            return BadRequest();

        var entity = OrderItemMapper.ToEntity(model);
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
