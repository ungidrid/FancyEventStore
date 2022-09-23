using AntActor.Core;
using FancyEventStore.DapperDummyStore;
using FancyEventStore.DapperProductionStore;
using FancyEventStore.DirectTests;
using FancyEventStore.DirectTests.Tests.Test1;
using FancyEventStore.Domain.TemperatureMeasurement;
using FancyEventStore.EfCoreStore;
using FancyEventStore.EventStore;
using FancyEventStore.EventStore.Abstractions;
using FancyEventStore.EventStore.Serializers;
using FancyEventStore.MongoDbStore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

internal class Program
{
    private const int retriesCount = 3;
    private static async Task Main(string[] args)
    {
        await Test1();
    
    }

    private static async Task Test1()
    {
        IServiceProvider provider;

        //Console.WriteLine("Start Mongo");
        //provider = ConfigureMongoServices();
        //var test1Mongo = ActivatorUtilities.CreateInstance<Test1Mongo>(provider, "test1_mongo.txt", retriesCount);
        //await test1Mongo.Run();

        //Console.WriteLine("Start Dapper");
        //provider = ConfigureDapperServices();
        //var test1Dapper = ActivatorUtilities.CreateInstance<Test1Dapper>(provider, "test1_dapper.txt", retriesCount);
        //await test1Dapper.Run();

        Console.WriteLine("Start Actor Dapper");
        provider = ConfigureDapperServices();
        var test1ActorDapper = ActivatorUtilities.CreateInstance<Test1ActorDapper>(provider, "test1_actor_dapper.txt", retriesCount);
        await test1ActorDapper.Run();

    }

    private static IServiceProvider ConfigureDapperServices()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddEventStore(Assembly.GetExecutingAssembly(),
            opts =>
            {
                opts.UseDapperStore(Configuration.SqlConnectionString);
                opts.EventSerializer = EventSerializers.Json;
                opts.SnapshotPredicate = null;
            },
            false);

        serviceCollection.AddTransient<IAntResolver, DIResolver>(provider => new DIResolver(provider));
        serviceCollection.AddScoped<Anthill>();


        return serviceCollection.BuildServiceProvider();
    }

    private static IServiceProvider ConfigureMongoServices()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddEventStore(Assembly.GetExecutingAssembly(),
            opts =>
            {
                opts.UseMongoDb(Configuration.MongoConnectionString);
                opts.EventSerializer = EventSerializers.Json;
                opts.SnapshotPredicate = null;
            },
            false);

        serviceCollection.AddTransient<IAntResolver, DIResolver>(provider => new DIResolver(provider));
        serviceCollection.AddScoped<Anthill>();

        return serviceCollection.BuildServiceProvider();
    }
}