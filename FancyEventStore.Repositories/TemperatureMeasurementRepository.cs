using FancyEventStore.Domain.TemperatureMeasurement;
using FancyEventStore.EventStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.Repositories
{
    public class TemperatureMeasurementRepository : ITemperatureMeasurementRepository
    {
        private readonly IEventStore _eventStore;

        public TemperatureMeasurementRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task AddMeasurementAsync(TemperatureMeasurement measurement)
        {
            await _eventStore.Store(measurement);
        }

        public async Task<TemperatureMeasurement> GetMeasurementAsync(Guid id)
        {
            return await _eventStore.Rehydrate<TemperatureMeasurement>(id);
        }

        public async Task UpdateMeasurementAsync(TemperatureMeasurement measurement)
        {
            await _eventStore.Store(measurement);
        }
    }
}
