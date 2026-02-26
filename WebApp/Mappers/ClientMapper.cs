using WebApp.Entities;
using WebApp.Models;

namespace WebApp.Mappers;

public static class ClientMapper
{
    public static Client ToModel(ClientEntity entity)
    {
        var model = new Client
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            Phone = entity.Phone
        };

        return model;
    }

    public static ClientEntity ToEntity(Client model)
    {
        var entity = new ClientEntity
        {
            Id = model.Id,
            Name = model.Name,
            Email = model.Email,
            Phone = model.Phone
        };

        return entity;
    }

    public static List<Client> ToModelList(IEnumerable<ClientEntity> entities)
    {
        var models = new List<Client>();

        foreach (var entity in entities)
        {
            var model = ToModel(entity);
            models.Add(model);
        }

        return models;
    }
}
