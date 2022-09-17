using FancyEventStore.EventStore;

namespace FancyEventStore.MongoDbStore
{
    internal static class MongoEntitiesMapper
    {
        public static Entities.EventStream ConvertToEntity(IEnumerable<Event> events)
        {
            var stream = events.FirstOrDefault().Stream;

            var eventStream = new Entities.EventStream
            {
                StreamId = stream.StreamId,
                Version = stream.Version,
                ConcurrencyToken = new Guid(stream.Timestamp ?? Guid.NewGuid().ToByteArray()),
                Events = events.Select(x => new Entities.Event
                {
                    StreamId = stream.StreamId,
                    Created = x.Created,
                    Type = x.Type,
                    Data = x.Data,
                    Version = x.Version
                }).ToList()
            };

            return eventStream;
        }

        public static Event ConvertToDomain(this Entities.Event @event)
        {
            return new Event
            {
                StreamId = @event.StreamId,
                Type = @event.Type,
                Created = @event.Created,
                Data = @event.Data,
                Version = @event.Version
            };
        }

        public static EventStream ConvertToDomain(this Entities.EventStream eventStream)
        {
            return new EventStream
            {
                StreamId = eventStream.StreamId,
                Version = eventStream.Version,
                Timestamp = eventStream.ConcurrencyToken.ToByteArray()
            };
        }
    }
}
