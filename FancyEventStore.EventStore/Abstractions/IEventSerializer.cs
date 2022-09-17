namespace FancyEventStore.EventStore.Abstractions
{
    public interface IEventSerializer
    {
        string Serialize(object @event);
        object Deserialize(string payload, Type eventType);
    }
}
