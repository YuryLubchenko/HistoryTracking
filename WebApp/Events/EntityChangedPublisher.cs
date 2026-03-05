using Microsoft.Extensions.DependencyInjection;
using WebApp.Entities;

namespace WebApp.Events;

public class EntityChangedPublisher
{
    private readonly IServiceProvider _serviceProvider;

    public EntityChangedPublisher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task PublishAsync<T>(EntityChangedEvent<T> entityChangedEvent) where T : BaseEntity
    {
        foreach (var handler in _serviceProvider.GetServices<IEntityChangedHandler<T>>())
            await handler.HandleAsync(entityChangedEvent);

        if (typeof(T) == typeof(BaseEntity))
            return;

        var baseEvent = new EntityChangedEvent<BaseEntity>
        {
            ActionType = entityChangedEvent.ActionType,
            OldEntity  = entityChangedEvent.OldEntity,
            NewEntity  = entityChangedEvent.NewEntity
        };

        foreach (var handler in _serviceProvider.GetServices<IEntityChangedHandler<BaseEntity>>())
            await handler.HandleAsync(baseEvent);
    }
}
