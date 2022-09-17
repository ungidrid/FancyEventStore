using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.MongoDbStore.Entities
{
    internal class EventStream
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public Guid StreamId { get; set; }
        public long Version { get; set; }

        public Guid ConcurrencyToken { get; set; }

        [BsonElement]
        public ICollection<Event> Events { get; set; }
    }
}
