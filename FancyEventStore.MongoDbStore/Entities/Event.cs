using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.MongoDbStore.Entities
{
    internal class Event
    {
        public Guid StreamId { get; set; }

        public DateTime Created { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public long Version { get; set; }
    }
}
