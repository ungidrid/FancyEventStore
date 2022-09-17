using FancyEventStore.EventStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FancyEventStore.EventStore.Serializers
{
    public class JsonEventSerializer : IEventSerializer
    {
        public object Deserialize(string payload, Type eventType)
        {
            return JsonSerializer.Deserialize(payload, eventType);
        }

        public string Serialize(object @event)
        {
            return JsonSerializer.Serialize(@event);
        }
    }
}
