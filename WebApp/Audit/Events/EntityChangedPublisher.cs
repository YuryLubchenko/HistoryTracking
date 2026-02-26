namespace WebApp.Audit.Events;

public class EntityChangedPublisher
{
    private readonly List<IEntityChangedHandler> _handlers = new();

    public void Subscribe(IEntityChangedHandler handler)
    {
        _handlers.Add(handler);
    }

    public async Task PublishAsync(EntityChangedEvent entityChangedEvent)
    {
        foreach (var handler in _handlers)
        {
            await handler.HandleAsync(entityChangedEvent);
        }
    }
}
