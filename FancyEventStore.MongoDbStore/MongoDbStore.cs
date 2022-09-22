using FancyEventStore.EventStore;
using FancyEventStore.EventStore.Snapshots;
using FancyEventStore.MongoDbStore.EntityProjection;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.MongoDbStore
{
    public class MongoDbStore : IStore
    {
        private readonly IMongoClient _mongoClient;

        public MongoDbStore(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
        }

        public async Task AppendEventsAsync(IEnumerable<Event> events)
        {
            var collection = GetEventStreamsCollection();
            var eventStream = MongoEntitiesMapper.ToEntity(events);

            var update = Builders<Entities.EventStream>.Update
                .Set(x => x.StreamId, eventStream.StreamId)
                .Set(x => x.Version, eventStream.Version)
                .Set(x => x.ConcurrencyToken, Guid.NewGuid())
                .PushEach(x => x.Events, eventStream.Events);

            var isUpsert = eventStream.Events.Count() - eventStream.Version == 0;

            var result = await collection.UpdateOneAsync(
                x => x.StreamId == eventStream.StreamId && x.ConcurrencyToken == eventStream.ConcurrencyToken, 
                update, 
                new UpdateOptions { IsUpsert = isUpsert }
            );

            if (!isUpsert && result.ModifiedCount == 0) throw new EventStoreConcurrencyException();
        }

        public async Task<IEnumerable<Event>> GetEventsAsync(Guid streamId, long? fromVersion = null, long? toVersion = null)
        {
            var collection = GetEventStreamsCollection();

            IEnumerable<Entities.Event> events = null;

            if(fromVersion == null && toVersion == null)
            {
                var stream = await GetStreamAsync(streamId, true);
                events = stream?.Events;
            }
            else
            {
                var stream = await collection.Aggregate()
                    .Match(x => x.StreamId == streamId)
                    .Unwind<Entities.EventStream, EventStreamEventFlattened>(x => x.Events)
                    .Match(x => (fromVersion == null || x.Events.Version >= fromVersion) 
                                && (toVersion == null || x.Events.Version <= toVersion))
                    .Group(x => x.StreamId, x => new FilteredEventsCollection { StreamId = x.Key, Events = x.Select(e => e.Events)})
                    .FirstOrDefaultAsync();

                events = stream?.Events;
            }

            return events?.Select(x => x.ToDomain()).ToList().AsEnumerable() ?? Enumerable.Empty<Event>();
        }

        public async Task<Snapshot> GetNearestSnapshotAsync(Guid streamId, long? version = null)
        {
            var collection = GetEventStreamsCollection();

            var snapshot = await collection.Aggregate()
                .Match(x => x.StreamId == streamId)
                .Unwind<Entities.EventStream, EventStreamSnapshotFlattened>(x => x.Snapshots)
                .Match(x => version == null || x.Snapshots.Version <= version)
                .SortByDescending(x => x.Snapshots.Version)
                .Project(x => new FilteredSnapshot { Id = x.Id, Snapshots = x.Snapshots})
                .FirstOrDefaultAsync();

            return snapshot?.Snapshots?.ToDomain();
        }

        public async Task<EventStream> GetStreamAsync(Guid streamId)
        {
            var eventStream = await GetStreamAsync(streamId, false);
            return eventStream?.ToDomain();
        }

        public async Task SaveSnapshot(Snapshot snapshot)
        {
            var collection = GetEventStreamsCollection();
            var snapshotEntity = snapshot.ToEntity();

            var update = Builders<Entities.EventStream>.Update
                .Push(x => x.Snapshots, snapshotEntity);

            await collection.UpdateOneAsync(x => x.StreamId == snapshot.StreamId, update);
        }

        private async Task<Entities.EventStream> GetStreamAsync(Guid streamId, bool includeEvents)
        {
            var collection = GetEventStreamsCollection();

            var filter = Builders<Entities.EventStream>.Filter.Eq(x => x.StreamId, streamId);
            var options = new FindOptions<Entities.EventStream>();

            if (!includeEvents)
            {
                options.Projection = Builders<Entities.EventStream>.Projection.Exclude(x => x.Events);
            };

            var eventStreamCollection = await collection.FindAsync(filter, options);
            var eventStream = await eventStreamCollection.FirstOrDefaultAsync();

            return eventStream;
        }

        private IMongoCollection<Entities.EventStream> GetEventStreamsCollection()
        {
            var db = _mongoClient.GetDatabase("EventStore");
            var collection = db.GetCollection<Entities.EventStream>("EventStream");

            return collection;
        }
    }
}
