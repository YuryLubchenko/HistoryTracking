using WebApp.Entities;

namespace WebApp.Events;

public class EntityChangedEvent<T> where T: BaseEntity
{
    public ActionType ActionType { get; set; }
    public T OldEntity { get; set; }
    public T NewEntity { get; set; }
}
