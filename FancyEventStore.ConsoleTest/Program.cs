using FancyEventStore.Domain.TemperatureMeasurement;
using FancyEventStore.EventStore;
using FancyEventStore.EventStore.Abstractions;
using FancyEventStore.EventStoreDb;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var connectionString = "esdb://localhost:2113?tls=false";
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddEventStoreDB(connectionString);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IEventStore>();

        var measurementId = Guid.Parse("19d3c70d-c10d-4958-a0ad-9e68a3d4ac3c");
        //var measurement = TemperatureMeasurement.Start(measurementId);
        //await store.Store(measurement);

        var measurement = await store.Rehydrate<TemperatureMeasurement>(measurementId);
    }
}