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
    private static IServiceCollection _services;
    private static IServiceProvider _provider;
    private static async Task Main(string[] args)
    {
        //Console.WriteLine("Start Mongo");
        //_services = ConfigureMongoServices();
        //_provider = _services.BuildServiceProvider();
        //var test1Mongo = ActivatorUtilities.CreateInstance<Test1Mongo>(_provider, "test1_mongo.txt");
        //await test1Mongo.Run();

        //Console.WriteLine("Start SQL");
        //_services = ConfigureEfServices();
        //_provider = _services.BuildServiceProvider();
        //var test1Ef = ActivatorUtilities.CreateInstance<Test1EF>(_provider, "test1_ef.txt");
        //await test1Ef.Run();

        //Console.WriteLine("Start Dummy Dapper");
        //_services = ConfigureDummyDapperServices();
        //_provider = _services.BuildServiceProvider();
        //var test1Dapper = ActivatorUtilities.CreateInstance<Test1Dapper>(_provider, "test1_dapper.txt", Configuration.UnsafeSqlConnectionString);
        //await test1Dapper.Run();

        Console.WriteLine("Start Dapper");
        _services = ConfigureDapperServices();
        _provider = _services.BuildServiceProvider();
        var test1Dapper = ActivatorUtilities.CreateInstance<Test1DummyDapper>(_provider, "test1_dapper.txt", Configuration.UnsafeSqlConnectionString);
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
}