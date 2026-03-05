using WebApp.Entities;

namespace WebApp.Events;

public interface IEntityChangedHandler<T> where T : BaseEntity
{
    Task HandleAsync(EntityChangedEvent<T> entityChangedEvent);
}
