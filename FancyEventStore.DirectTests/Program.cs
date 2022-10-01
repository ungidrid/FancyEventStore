using AntActor.Core;
using FancyEventStore.DapperProductionStore;
using FancyEventStore.DirectTests;
using FancyEventStore.DirectTests.Tests.Test1;
using FancyEventStore.DirectTests.Tests.Test2;
using FancyEventStore.DirectTests.Tests.Test3;
using FancyEventStore.DirectTests.Tests.Test4;
using FancyEventStore.DirectTests.Tests.Test5;
using FancyEventStore.EventStore;
using FancyEventStore.EventStore.Serializers;
using FancyEventStore.EventStoreDb;
using FancyEventStore.MongoDbStore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

internal class Program
{
    private const int retriesCount = 10;
    private static async Task Main(string[] args)
    {
        await Test5();
    }

    private static async Task Test1()
    {
        IServiceProvider provider;

        Console.WriteLine("Start Mongo");
        provider = ConfigureMongoServices();
        var test1Mongo = ActivatorUtilities.CreateInstance<Test1Mongo>(provider, "results/test1_mongo.txt", retriesCount);
        await test1Mongo.Run();

        Console.WriteLine("Start Dapper");
        provider = ConfigureDapperServices();
        var test1Dapper = ActivatorUtilities.CreateInstance<Test1Dapper>(provider, "results/test1_dapper.txt", retriesCount);
        await test1Dapper.Run();

        Console.WriteLine("Start Actor Dapper");
        provider = ConfigureDapperServices();
        var test1ActorDapper = ActivatorUtilities.CreateInstance<Test1ActorDapper>(provider, "results/test1_actor_dapper.txt", retriesCount);
        await test1ActorDapper.Run();
    }

    private static async Task Test2()
    {
        IServiceProvider provider;

        Console.WriteLine("Start Mongo");
        provider = ConfigureMongoServices();
        var test2Mongo = ActivatorUtilities.CreateInstance<Test2Mongo>(provider, "results/test2_mongo.txt", retriesCount);
        await test2Mongo.Run();

        Console.WriteLine("Start Dapper");
        provider = ConfigureDapperServices();
        var test2Dapper = ActivatorUtilities.CreateInstance<Test2Dapper>(provider, "results/test2_dapper.txt", retriesCount);
        await test2Dapper.Run();

        Console.WriteLine("Start Actor Dapper");
        provider = ConfigureDapperServices();
        var test2ActorDapper = ActivatorUtilities.CreateInstance<Test2ActorDapper>(provider, "results/test2_actor_dapper.txt", retriesCount);
        await test2ActorDapper.Run();
    }

    private static async Task Test3()
    {
        IServiceProvider provider;

        Console.WriteLine("Start Mongo");
        provider = ConfigureMongoServices();
        var test3Mongo = ActivatorUtilities.CreateInstance<Test3Mongo>(provider, "results/test3_mongo.txt", retriesCount);
        await test3Mongo.Run();

        Console.WriteLine("Start Dapper");
        provider = ConfigureDapperServices();
        var test3Dapper = ActivatorUtilities.CreateInstance<Test3Dapper>(provider, "results/test3_dapper.txt", retriesCount);
        await test3Dapper.Run();

        Console.WriteLine("Start Actor Dapper");
        provider = ConfigureDapperServices();
        var test3ActorDapper = ActivatorUtilities.CreateInstance<Test3ActorDapper>(provider, "results/test3_actor_dapper.txt", retriesCount);
        await test3ActorDapper.Run();
    }

    private static async Task Test4()
    {
        IServiceProvider provider;

        Console.WriteLine("Start Mongo");
        provider = ConfigureMongoServices();
        var test4Mongo = ActivatorUtilities.CreateInstance<Test4Mongo>(provider, "results/test4_mongo.txt", retriesCount);
        await test4Mongo.Run();

        Console.WriteLine("Start Dapper");
        provider = ConfigureDapperServices();
        var test4Dapper = ActivatorUtilities.CreateInstance<Test4Dapper>(provider, "results/test4_dapper.txt", retriesCount);
        await test4Dapper.Run();

        Console.WriteLine("Start Actor Dapper");
        provider = ConfigureDapperServices();
        var test4ActorDapper = ActivatorUtilities.CreateInstance<Test4ActorDapper>(provider, "results/test4_actor_dapper.txt", retriesCount);
        await test4ActorDapper.Run();

        Console.WriteLine("Start EventStoreDb");
        provider = ConfigureEventStoreDbServices();
        var test4EventStoreDb = ActivatorUtilities.CreateInstance<Test4EventStoreDb>(provider, "results/test4_eventStoreDb.txt", retriesCount);
        await test4EventStoreDb.Run();
    }

    private static async Task Test5()
    {
        IServiceProvider provider;
        int threadsCount = 5;

        Console.WriteLine("Start Mongo");
        provider = ConfigureMongoServices();
        var test5Mongo = ActivatorUtilities.CreateInstance<Test5Mongo>(provider, "results/test5_mongo.txt", threadsCount);
        await test5Mongo.Run();

        Console.WriteLine("Start Dapper");
        provider = ConfigureDapperServices();
        var test5Dapper = ActivatorUtilities.CreateInstance<Test5Dapper>(provider, "results/test5_dapper.txt", threadsCount);
        await test5Dapper.Run();

        Console.WriteLine("Start Actor Dapper");
        provider = ConfigureDapperServices();
        var test5ActorDapper = ActivatorUtilities.CreateInstance<Test5ActorDapper>(provider, "results/test5_actor_dapper.txt", threadsCount);
        await test5ActorDapper.Run();

        Console.WriteLine("Start EventStoreDb");
        provider = ConfigureEventStoreDbServices();
        var test5EventStoreDb = ActivatorUtilities.CreateInstance<Test5EventStoreDb>(provider, "results/test5_eventStoreDb.txt", threadsCount);
        await test5EventStoreDb.Run();
    }


    private static IServiceProvider ConfigureDapperServices()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddEventStore(Assembly.GetExecutingAssembly(),
            opts =>
            {
                //opts.UseEfCore(dbContextOptions => dbContextOptions.UseSqlServer(Configuration.SqlConnectionString));
                opts.UseDapperStore(Configuration.SqlConnectionString);
                opts.EventSerializer = EventSerializers.Json;
                opts.SnapshotPredicate = null;// new EachNEventsSnapshotPredicate(500);
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
                opts.SnapshotPredicate = null;// new EachNEventsSnapshotPredicate(500);
            },
            false);

        serviceCollection.AddTransient<IAntResolver, DIResolver>(provider => new DIResolver(provider));
        serviceCollection.AddScoped<Anthill>();

        return serviceCollection.BuildServiceProvider();
    }

    private static IServiceProvider ConfigureEventStoreDbServices()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddEventStoreDB(Configuration.EventStoreDbConnectionString);
        return serviceCollection.BuildServiceProvider();
    }
}