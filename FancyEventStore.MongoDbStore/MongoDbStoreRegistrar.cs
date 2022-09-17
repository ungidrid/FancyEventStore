using FancyEventStore.EventStore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.MongoDbStore
{
    public class MongoDbStoreRegistrar : IStoreRegistrar
    {
        private readonly string _connectionString;

        public MongoDbStoreRegistrar(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void RegisterStore(IServiceCollection services)
        {
            services.AddScoped<IMongoClient>(_ => new MongoClient(_connectionString));
            services.AddScoped<IStore, MongoDbStore>();
        }
    }
}
