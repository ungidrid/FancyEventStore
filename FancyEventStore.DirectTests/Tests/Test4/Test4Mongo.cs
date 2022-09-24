using FancyEventStore.EventStore.Abstractions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.DirectTests.Tests.Test4
{
    public class Test4Mongo: Test4Base
    {
        private readonly IMongoClient _mongoClient;

        public Test4Mongo(IEventStore eventStore, IMongoClient mongoClient, string resultFileName, int retriesCount) : base(eventStore, resultFileName, retriesCount)
        {
            _mongoClient = mongoClient;
        }

        protected override async Task CleanData()
        {
            var db = _mongoClient.GetDatabase("EventStore");
            var collection = db.GetCollection<MongoDbStore.Entities.EventStream>("EventStream");

            await collection.DeleteManyAsync(x => true);
        }
    }
}
