using AntActor.Core;
using FancyEventStore.DapperDummyStore;
using FancyEventStore.DapperProductionStore;
using FancyEventStore.DirectTests;
using FancyEventStore.DirectTests.Tests.Test1;
using FancyEventStore.DirectTests.Tests.Test2;
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
    private const int retriesCount = 10;
    private static async Task Main(string[] args)
    {
        await Test2();
    }

    private static async Task Test1()
    {
        IServiceProvider provider;

        Console.WriteLine("Start Mongo");
        provider = ConfigureMongoServices();
        var test1Mongo = ActivatorUtilities.CreateInstance<Test1Mongo>(provider, "test1_mongo.txt", retriesCount);
        await test1Mongo.Run();

        Console.WriteLine("Start Dapper");
        provider = ConfigureDapperServices();
        var test1Dapper = ActivatorUtilities.CreateInstance<Test1Dapper>(provider, "test1_dapper.txt", retriesCount);
        await test1Dapper.Run();

        Console.WriteLine("Start Actor Dapper");
        provider = ConfigureDapperServices();
        var test1ActorDapper = ActivatorUtilities.CreateInstance<Test1ActorDapper>(provider, "test1_actor_dapper.txt", retriesCount);
        await test1ActorDapper.Run();
    }

    private static async Task Test2()
    {
        IServiceProvider provider;

        Console.WriteLine("Start Mongo");
        provider = ConfigureMongoServices();
        var test2Mongo = ActivatorUtilities.CreateInstance<Test2Mongo>(provider, "test2_mongo.txt", retriesCount);
        await test2Mongo.Run();

        Console.WriteLine("Start Dapper");
        provider = ConfigureDapperServices();
        var test2Dapper = ActivatorUtilities.CreateInstance<Test2Dapper>(provider, "test2_dapper.txt", retriesCount);
        await test2Dapper.Run();

        Console.WriteLine("Start Actor Dapper");
        provider = ConfigureDapperServices();
        var test2ActorDapper = ActivatorUtilities.CreateInstance<Test2ActorDapper>(provider, "test2_actor_dapper.txt", retriesCount);
        await test2ActorDapper.Run();
    }

    private static IServiceProvider ConfigureDapperServices()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddEventStore(Assembly.GetExecutingAssembly(),
            opts =>
            {
                opts.UseDapperStore(Configuration.SqlConnectionString);
                opts.EventSerializer = EventSerializers.MessagePack;
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
                opts.EventSerializer = EventSerializers.MessagePack;
                opts.SnapshotPredicate = null;
            },
            false);

        serviceCollection.AddTransient<IAntResolver, DIResolver>(provider => new DIResolver(provider));
        serviceCollection.AddScoped<Anthill>();

        return serviceCollection.BuildServiceProvider();
    }
}