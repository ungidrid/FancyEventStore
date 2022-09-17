using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.Domain.TemperatureMeasurement.Events
{
    public record TemperatureRecorded(
        Guid MeasurementId,
        decimal Temperature,
        DateTimeOffset MeasuredAt
    )
    {
        public static TemperatureRecorded Create(Guid measurementId, decimal temperature)
        {
            if (temperature < -273)
                throw new ArgumentOutOfRangeException(nameof(temperature));

            return new TemperatureRecorded(measurementId, temperature, DateTimeOffset.UtcNow);
        }
    }

}
