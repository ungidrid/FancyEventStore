using FancyEventStore.EventStore.Abstractions;
using MessagePack;

namespace FancyEventStore.EventStore.Serializers
{
    public class MessagePackEventSerializer : IEventSerializer
    {
        public object Deserialize(string payload, Type eventType)
        {
            var bytes = Convert.FromBase64String(payload);
            return MessagePackSerializer.Deserialize(eventType, bytes, MessagePack.Resolvers.ContractlessStandardResolver.Options);
        }

        public string Serialize(object @event)
        {
            var serialized = MessagePackSerializer.Serialize(@event, MessagePack.Resolvers.ContractlessStandardResolver.Options);
            return Convert.ToBase64String(serialized);
        }
    }
}
