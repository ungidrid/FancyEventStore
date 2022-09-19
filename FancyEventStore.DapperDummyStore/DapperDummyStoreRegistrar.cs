using FancyEventStore.EventStore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.DapperDummyStore
{
    internal class DapperDummyStoreRegistrar: IStoreRegistrar
    {
        private readonly string _connectionString;

        public DapperDummyStoreRegistrar(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void RegisterStore(IServiceCollection services)
        {
            services.AddScoped<IStore>(_ => new DapperDummyStore(_connectionString));
        }
    }
}
