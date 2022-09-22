using FancyEventStore.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("FancyEventStore.DirectTests")]
namespace FancyEventStore.MongoDbStore
{
    public static class MongoEventStoreBuilderExternsions
    {
        public static EventStoreOptions UseMongoDb(this EventStoreOptions optionsBuilder, string connectionString)
        {
            optionsBuilder.StoreRegistrar = new MongoDbStoreRegistrar(connectionString);
            return optionsBuilder;
        }
    }
}
