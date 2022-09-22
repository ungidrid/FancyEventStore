using FancyEventStore.DapperProductionStore;
using FancyEventStore.EventStore;
using System.ComponentModel.DataAnnotations;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var connectionString = "Data Source=LAPTOP-DDOSKACH;Initial Catalog = FancyEventStoreDb; Integrated Security = True; Connect Timeout = 3600";
        var context = new DbContext(connectionString);
        var store = new DapperProductionStore(context);

        var stream = await store.GetStreamAsync(Guid.Parse("6B8F6A94-4C0E-4080-9CE2-B62A1E5C702E"));

        var e = new Event
        {
            StreamId = stream.StreamId,
            Type = "Type",
            Data = "Data",
            Version = 4
        };
        stream.Version = 4;

        await store.AppendEventsAsync(stream, new[] { e });
        await store.AppendEventsAsync(stream, new[] { e });
    
    }
}