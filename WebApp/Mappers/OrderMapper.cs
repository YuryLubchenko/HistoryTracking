using WebApp.Entities;
using WebApp.Models;

namespace WebApp.Mappers;

public static class OrderMapper
{
    public static Order ToModel(OrderEntity entity)
    {
        var model = new Order
        {
            Id = entity.Id,
            ClientId = entity.ClientId,
            OrderDate = entity.OrderDate,
            TotalAmount = entity.TotalAmount
        };

        return model;
    }

    public static OrderEntity ToEntity(Order model)
    {
        var entity = new OrderEntity
        {
            Id = model.Id,
            ClientId = model.ClientId,
            OrderDate = model.OrderDate,
            TotalAmount = model.TotalAmount
        };

        return entity;
    }

    public static List<Order> ToModelList(IEnumerable<OrderEntity> entities)
    {
        var models = new List<Order>();

        foreach (var entity in entities)
        {
            var model = ToModel(entity);
            models.Add(model);
        }

        return models;
    }
}
