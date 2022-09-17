using FancyEventStore.Domain.TemperatureMeasurement;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.Repositories
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ITemperatureMeasurementRepository, TemperatureMeasurementRepository>();

            return serviceCollection;
        }
    }
}
