namespace FancyEventStore.Api.Models
{
    public class TemperatureMeasurementShort
    {
        public Guid Id { get; set; }
        public DateTimeOffset Started { get; set; }
        public DateTimeOffset? LastRecorded { get; set; }
        public int? MeasurementsCount { get; set; }
        public decimal? Average { get; set; }
    }
}
 