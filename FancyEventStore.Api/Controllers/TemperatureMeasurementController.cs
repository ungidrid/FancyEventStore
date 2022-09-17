using FancyEventStore.Api.Models;
using FancyEventStore.Domain.TemperatureMeasurement;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;
using FancyEventStore.EventStore.Abstractions;
using FancyEventStore.ReadModel;

namespace FancyEventStore.Api.Controllers
{
    [ApiController]
    [Route("api/temperature-measurement")]
    public class TemperatureMeasurementController: ControllerBase
    {
        private readonly ITemperatureMeasurementRepository _temperatureMeasurementRepository;
        private readonly IReadModelContext _readDbConnection;

        public TemperatureMeasurementController(
            ITemperatureMeasurementRepository temperatureMeasurementRepository, 
            IReadModelContext readDbConnection)
        {
            _temperatureMeasurementRepository = temperatureMeasurementRepository;
            _readDbConnection = readDbConnection;
        }

        [HttpGet("{id}")]
        public async Task<TemperatureMeasurementShort> Get(Guid id)
        {
            var sql = "SELECT TOP(1) * " +
                      "FROM TemperatureMeasurementShort " +
                      "WHERE Id = @id";

            using var connection = _readDbConnection.Connection;
            return await connection.QueryFirstAsync<TemperatureMeasurementShort>(sql, new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Start()
        {
            var id = Guid.NewGuid();
            var measurement = TemperatureMeasurement.Start(id);
            await _temperatureMeasurementRepository.AddMeasurementAsync(measurement);

            return Created("api/TemperatureMeasurements", id);
        }


        [HttpPost("{id}/temperatures")]
        public async Task<IActionResult> Record(Guid id, [FromBody] decimal temperature)
        {
            var measurement = await _temperatureMeasurementRepository.GetMeasurementAsync(id);
            measurement.Record(temperature);
            await _temperatureMeasurementRepository.UpdateMeasurementAsync(measurement);

            return Ok();
        }
    }
}
