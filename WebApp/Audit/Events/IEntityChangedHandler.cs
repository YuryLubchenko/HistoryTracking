namespace WebApp.Audit.Events;

public interface IEntityChangedHandler
{
    Task HandleAsync(EntityChangedEvent entityChangedEvent);
}
