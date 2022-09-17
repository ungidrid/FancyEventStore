using FancyEventStore.Domain.TemperatureMeasurement.Events;
using FancyEventStore.EventStore.Abstractions;
using System.Data;
using Dapper;
using FancyEventStore.ReadModel;

namespace FancyEventStore.Api.Projections
{
    public class TemperatureMeasurementShortProjection : IProjection
    {
        private const string ProjectionTarget = "TemperatureMeasurementShort";

        private readonly IReadOnlyList<Type> _handlableEventTypes;
        private readonly IReadModelContext _context;

        public TemperatureMeasurementShortProjection(IReadModelContext context)
        {
            _handlableEventTypes = new[] { 
                typeof(TemperatureMeasurementStarted), 
                typeof(TemperatureRecorded) 
            };
            
            _context = context;

            using var connection = _context.Connection;
            connection.Execute(@$"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{ProjectionTarget}' and xtype='U')
                    CREATE TABLE {ProjectionTarget}(
                        Id UNIQUEIDENTIFIER NOT NULL,
                        Started DATETIME NOT NULL,
                        LastRecorded DATETIME NULL,
                        MeasurementsCount INT NULL,
                        Average decimal(12,8) NULL
                    )
               ");
        }

        public bool CanHandle(object @event)
        {
            return _handlableEventTypes.Contains(@event.GetType());
        }

        public async Task WhenAsync(object @event)
        {
            var applyTask = @event switch
            {
                TemperatureMeasurementStarted e => ApplyAsync(e),
                TemperatureRecorded e => ApplyAsync(e)
            };

            await applyTask;
        }

        private async Task ApplyAsync(TemperatureMeasurementStarted @event)
        {
            var sql = @$"INSERT INTO {ProjectionTarget}(Id, Started) 
                            VALUES (@MeasurementId, @StartedAt);";

            using var connection = _context.Connection;
            await connection.ExecuteAsync(sql, @event);
        }

        private async Task ApplyAsync(TemperatureRecorded @event)
        {
            var sql = $@"UPDATE {ProjectionTarget} 
                         SET LastRecorded = @MeasuredAt, 
                             Average = (ISNULL(Average, 0) * ISNULL(MeasurementsCount, 0) + @Temperature)/(ISNULL(MeasurementsCount, 0) + 1),
                             MeasurementsCount = ISNULL(MeasurementsCount, 0) + 1
                         WHERE Id = @MeasurementId;";

            using var connection = _context.Connection;
            await connection.ExecuteAsync(sql, @event);
        }
    }
}