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
        public void RegisterStore(IServiceCollection services)
        {
            services.AddScoped<IMongoClient>(_ => new MongoClient("mongodb+srv://root:prokiller00@eventstore.rf4fems.mongodb.net/test"));
        }
    }
}
