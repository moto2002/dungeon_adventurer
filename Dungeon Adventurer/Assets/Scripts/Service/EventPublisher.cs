public abstract class EventPublisher
{
    public abstract void RegisterListener(EventListener listener);
    public abstract void UnregisterListener(EventListener listener);
    public abstract void Publish();
    public abstract void Init();
}
