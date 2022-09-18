using FancyEventStore.MongoDbStore.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FancyEventStore.MongoDbStore.EntityProjection
{
    internal class EventStreamSnapshotFlattened
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement]
        public Snapshot Snapshots { get; set; }
    }
}
