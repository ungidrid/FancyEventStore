using FancyEventStore.MongoDbStore.Entities;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.MongoDbStore.EntityProjection
{
    internal class FilteredEventsCollection
    {
        [BsonId]
        public Guid StreamId { get; set; }

        public IEnumerable<Event> Events { get; set; }
    }
}
