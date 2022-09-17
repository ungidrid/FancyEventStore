using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FancyEventStore.Domain.TemperatureMeasurement.Events
{
    public class TemperatureMeasurementStarted
    {
        public Guid MeasurementId { get; }
        public DateTimeOffset StartedAt { get; }

        [JsonConstructor]
        public TemperatureMeasurementStarted(Guid measurementId, DateTimeOffset startedAt)
        {
            MeasurementId = measurementId;
            StartedAt = startedAt;
        }

        public static TemperatureMeasurementStarted Create(Guid measurementId)
        {
            return new TemperatureMeasurementStarted(measurementId, DateTimeOffset.UtcNow);
        }
    }
}
