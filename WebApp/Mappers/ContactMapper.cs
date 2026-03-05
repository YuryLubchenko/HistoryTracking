using WebApp.Entities;
using WebApp.Models;

namespace WebApp.Mappers;

public static class ContactMapper
{
    public static Contact ToModel(ContactEntity entity)
    {
        var model = new Contact
        {
            Id = entity.Id,
            ContactType = (Models.ContactType)entity.ContactType,
            Value = entity.Value
        };

        return model;
    }

    public static ContactEntity ToEntity(Contact model, long clientId)
    {
        var entity = new ContactEntity
        {
            Id = model.Id,
            ClientId = clientId,
            ContactType = (Entities.ContactType)model.ContactType,
            Value = model.Value
        };

        return entity;
    }

    public static List<Contact> ToModelList(IEnumerable<ContactEntity> entities)
    {
        var models = new List<Contact>();

        foreach (var entity in entities)
        {
            var model = ToModel(entity);
            models.Add(model);
        }

        return models;
    }
}
