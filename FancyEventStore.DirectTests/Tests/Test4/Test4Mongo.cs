using FancyEventStore.EventStore.Abstractions;
using MongoDB.Driver;

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

            var snapshpts = db.GetCollection<MongoDbStore.Entities.Snapshot>("Snapshots");
            await snapshpts.DeleteManyAsync(x => true);
        }
    }
}
