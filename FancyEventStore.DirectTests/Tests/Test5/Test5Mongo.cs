using FancyEventStore.EventStore.Abstractions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.DirectTests.Tests.Test5
{
    public class Test5Mongo: Test5Base
    {
        private readonly IMongoClient _mongoClient;

        public Test5Mongo(IServiceProvider eventStore, IMongoClient mongoClient, string resultFileName, int retriesCount) : base(eventStore, resultFileName, retriesCount)
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
