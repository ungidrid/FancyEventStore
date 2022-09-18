using FancyEventStore.MongoDbStore.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.MongoDbStore.EntityProjection
{
    internal class EventStreamEventFlattened
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public Guid StreamId { get; set; }
        public Event Events { get; set; }
    }
}
