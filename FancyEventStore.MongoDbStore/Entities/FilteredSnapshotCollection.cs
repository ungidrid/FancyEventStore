using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FancyEventStore.MongoDbStore.Entities
{
    internal class FilteredSnapshot
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public Snapshot Snapshot { get; set; }
    }
}
