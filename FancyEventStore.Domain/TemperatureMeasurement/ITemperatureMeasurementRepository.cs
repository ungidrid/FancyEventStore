
namespace FancyEventStore.Domain.TemperatureMeasurement
{
    public interface ITemperatureMeasurementRepository
    {
        Task<TemperatureMeasurement> GetMeasurementAsync(Guid id);
        Task AddMeasurementAsync(TemperatureMeasurement measurement);
        Task UpdateMeasurementAsync(TemperatureMeasurement measurement);
    }
}
