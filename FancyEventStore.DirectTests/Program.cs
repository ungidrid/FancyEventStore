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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

internal class Program
{
    private const int retriesCount = 10;
    private static async Task Main(string[] args)
    {
        
    
    }

    private static async Task Test1()
    {
        Console.WriteLine("Start Mongo");
        var services = ConfigureMongoServices();
        var provider = services.BuildServiceProvider();
        var test1Mongo = ActivatorUtilities.CreateInstance<Test1Mongo>(provider, "test1_mongo.txt", retriesCount);
        await test1Mongo.Run();

        Console.WriteLine("Start SQL");
        services = ConfigureEfServices();
        provider = services.BuildServiceProvider();
        var test1Ef = ActivatorUtilities.CreateInstance<Test1EF>(provider, "test1_ef.txt", retriesCount);
        await test1Ef.Run();

        Console.WriteLine("Start Dapper");
        services = ConfigureDapperServices();
        provider = services.BuildServiceProvider();
        var test1Dapper = ActivatorUtilities.CreateInstance<Test1Dapper>(provider, "test1_dapper.txt", retriesCount);
        await test1Dapper.Run();

        Console.WriteLine("Start Dummy Dapper");
        services = ConfigureDummyDapperServices();
        provider = services.BuildServiceProvider();
        var test1DummyDapper = ActivatorUtilities.CreateInstance<Test1DummyDapper>(provider, "test1_dummy_dapper.txt", Configuration.UnsafeSqlConnectionString, retriesCount);
        await test1DummyDapper.Run();

        Console.WriteLine("Start actor");
        services = ConfigureActorSqlServices();
        provider = services.BuildServiceProvider();
    }

    private static IServiceCollection ConfigureEfServices()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddEventStore(Assembly.GetExecutingAssembly(),
            opts =>
            {
                opts.UseEfCore(dbContextOptions => dbContextOptions.UseSqlServer(Configuration.SqlConnectionString));
                opts.EventSerializer = EventSerializers.Json;
                opts.SnapshotPredicate = null;
            },
            false);

        return serviceCollection;
    }

    private static IServiceCollection ConfigureDapperServices()
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


        return serviceCollection;
    }

    private static IServiceCollection ConfigureMongoServices()
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

        return serviceCollection;
    }

    private static IServiceCollection ConfigureDummyDapperServices()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddEventStore(Assembly.GetExecutingAssembly(),
            opts =>
            {
                opts.UseDummyDapperStore(Configuration.UnsafeSqlConnectionString);
                opts.EventSerializer = EventSerializers.Json;
                opts.SnapshotPredicate = null;
            },
            true);

        return serviceCollection;
    }

    private static IServiceCollection ConfigureActorSqlServices()
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


        return serviceCollection;
    }
}