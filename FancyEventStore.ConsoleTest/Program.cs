using FancyEventStore.EventStore;
using FancyEventStore.MongoDbStore;
using MongoDB.Driver;

var mongoClient = new MongoClient("mongodb+srv://root:prokiller00@eventstore.rf4fems.mongodb.net/test");
var eventStore = new MongoDbStore(mongoClient);

var streamId = Guid.Parse("78f65243-0114-483b-827b-ade710b7975f");
var events = await eventStore.GetEventsAsync(streamId, 2, 5);

Console.ReadLine();

int i, a = 0;

