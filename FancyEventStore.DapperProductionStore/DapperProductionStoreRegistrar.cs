using FancyEventStore.EventStore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.DapperProductionStore
{
    internal class DapperProductionStoreRegistrar : IStoreRegistrar
    {
        private readonly string _connectionString;

        public DapperProductionStoreRegistrar(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void RegisterStore(IServiceCollection services)
        {
            services.AddScoped<IStore, DapperProductionStore>();
            services.AddScoped<IDbContext>(_ => new DbContext(_connectionString));
        }
    }
}
