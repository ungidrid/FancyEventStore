using FancyEventStore.EventStore;
using FancyEventStore.EventStore.Snapshots;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var eventStream = MongoEntitiesMapper.ConvertToEntity(events);

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
                    .Unwind(x => x.Events)
                    .Match(new BsonDocument("$and", 
                        new BsonArray
                        {
                            new BsonDocument("Events.Version", new BsonDocument("$gte", fromVersion ?? 0)),
                            new BsonDocument("Events.Version", new BsonDocument("$lte", toVersion ?? long.MaxValue))
                        }))
                    .Group(new BsonDocument{
                        { "_id", "$Events.StreamId" },
                        { "Events", new BsonDocument("$push", "$Events")}
                    })
                    .Project<Entities.FilteredEventsCollection>(new BsonDocument("Events", 1))
                    .FirstOrDefaultAsync();

                events = stream?.Events;
            }

            return events?.Select(x => x.ConvertToDomain()).ToList();
        }

        public async Task<Snapshot> GetNearestSnapshotAsync(Guid streamId, long? version = null)
        {
            return null;
        }

        public async Task<EventStream> GetStreamAsync(Guid streamId)
        {
            var eventStream = await GetStreamAsync(streamId, false);
            return eventStream?.ConvertToDomain();
        }

        public Task SaveSnapshot(Snapshot snapshot)
        {
            return Task.CompletedTask;
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
