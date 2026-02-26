using WebApp.Entities;
using WebApp.Models;

namespace WebApp.Mappers;

public static class OrderItemMapper
{
    public static OrderItem ToModel(OrderItemEntity entity)
    {
        var model = new OrderItem
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            ProductName = entity.ProductName,
            Quantity = entity.Quantity,
            UnitPrice = entity.UnitPrice
        };

        return model;
    }

    public static OrderItemEntity ToEntity(OrderItem model)
    {
        var entity = new OrderItemEntity
        {
            Id = model.Id,
            OrderId = model.OrderId,
            ProductName = model.ProductName,
            Quantity = model.Quantity,
            UnitPrice = model.UnitPrice
        };

        return entity;
    }

    public static List<OrderItem> ToModelList(IEnumerable<OrderItemEntity> entities)
    {
        var models = new List<OrderItem>();

        foreach (var entity in entities)
        {
            var model = ToModel(entity);
            models.Add(model);
        }

        return models;
    }
}
