namespace WebApp.Events;

public class EntityChangedEvent
{
    public ActionType ActionType { get; set; }
    public object OldEntity { get; set; }
    public object NewEntity { get; set; }
}
