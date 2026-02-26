namespace WebApp.Events;

public interface IEntityChangedHandler
{
    Task HandleAsync(EntityChangedEvent entityChangedEvent);
}
