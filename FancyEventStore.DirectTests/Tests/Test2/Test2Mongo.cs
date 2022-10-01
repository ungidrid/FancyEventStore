using FancyEventStore.DirectTests.Tests.Test3;
using FancyEventStore.EventStore.Abstractions;
using MongoDB.Driver;

namespace FancyEventStore.DirectTests.Tests.Test2
{
    public class Test3Mongo : Test3Base
    {
        private readonly IMongoClient _mongoClient;

        public Test3Mongo(IEventStore eventStore, IMongoClient mongoClient, string resultFileName, int retriesCount) : base(eventStore, resultFileName, retriesCount)
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
