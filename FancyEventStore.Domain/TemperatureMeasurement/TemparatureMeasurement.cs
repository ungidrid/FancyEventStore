using FancyEventStore.Domain.Abstract;
using FancyEventStore.Domain.TemperatureMeasurement.Events;

namespace FancyEventStore.Domain.TemperatureMeasurement
{
    public class TemperatureMeasurement : Aggregate
    {
        public DateTimeOffset Started { get; set; }
        public DateTimeOffset? LastRecorded { get; set; }
        public List<decimal> Mesurements { get; set; } = default!;

        public TemperatureMeasurement()
        {
        }

        private TemperatureMeasurement(Guid id)
        {
            var @event = TemperatureMeasurementStarted.Create(id);

            Enqueue(@event);
            Apply(@event);
        }

        public static TemperatureMeasurement Start(Guid id)
        {
            return new TemperatureMeasurement(id);
        }

        public void Record(decimal temperature)
        {
            if (temperature < -273)
                throw new ArgumentOutOfRangeException(nameof(temperature));

            var @event = TemperatureRecorded.Create(Id, temperature);

            Enqueue(@event);
            Apply(@event);
        }

        public override void When(object @event)
        {
            switch (@event)
            {
                case TemperatureMeasurementStarted x:
                    Apply(x);
                    break;
                case TemperatureRecorded x:
                    Apply(x);
                    break;
            }
        }

        public void Apply(TemperatureMeasurementStarted @event)
        {
            Id = @event.MeasurementId;
            Started = @event.StartedAt;
            Mesurements = new List<decimal>();
        }

        public void Apply(TemperatureRecorded @event)
        {
            Mesurements.Add(@event.Temperature);
            LastRecorded = @event.MeasuredAt;
        }
    }
}
