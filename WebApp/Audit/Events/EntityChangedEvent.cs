namespace WebApp.Audit.Events;

public enum ActionType
{
    Created,
    Updated,
    Deleted
}

public class EntityChangedEvent
{
    public ActionType ActionType { get; set; }
    public object OldEntity { get; set; }
    public object NewEntity { get; set; }
}
