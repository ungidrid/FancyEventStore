using FancyEventStore.DirectTests.Tests.Test2;
using FancyEventStore.EventStore.Abstractions;
using MongoDB.Driver;

namespace FancyEventStore.DirectTests.Tests.Test3
{
    public class Test2Mongo : Test3Base
    {
        private readonly IMongoClient _mongoClient;

        public Test2Mongo(IEventStore eventStore, IMongoClient mongoClient, string resultFileName, int retriesCount) : base(eventStore, resultFileName, retriesCount)
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
