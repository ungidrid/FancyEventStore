using FancyEventStore.EventStore.Abstractions;

namespace FancyEventStore.EventStore.Serializers
{
    public class ProtobufEventSerializer : IEventSerializer
    {
        public object Deserialize(string payload, Type eventType)
        {
            var bytes = Convert.FromBase64String(payload);
            var ms = new MemoryStream(bytes);
            return ProtoBuf.Serializer.Deserialize(eventType, ms);
        }

        public string Serialize(object @event)
        {
            var ms = new MemoryStream();
            ProtoBuf.Serializer.Serialize(ms, @event);

            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
