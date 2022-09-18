using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.MongoDbStore.Entities
{
    internal class Snapshot
    {
        [BsonId]
        public ObjectId SnapshotId { get; set; }
        public Guid StreamId { get; set; }

        [BsonElement]
        public string Data { get; set; }
        public long Version { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
