using EventStore.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.EventStoreDb.Helpers
{
    internal static class EventStoreDBSerializer
    {
        public static T? Deserialize<T>(this ResolvedEvent resolvedEvent) where T : class =>
            resolvedEvent.Deserialize() as T;

        public static object? Deserialize(this ResolvedEvent resolvedEvent)
        {
            // get type
            var eventType = EventTypeMapper.ToType(resolvedEvent.Event.EventType);

            return eventType != null
                // deserialize event
                ? JsonConvert.DeserializeObject(Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span), eventType)
                : null;
        }

        public static EventData ToJsonEventData(this object @event) =>
            new(
                Uuid.NewUuid(),
                EventTypeMapper.ToName(@event.GetType()),
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { }))
            );
    }
}
