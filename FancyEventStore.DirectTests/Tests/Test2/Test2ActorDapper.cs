using AntActor.Core;
using FancyEventStore.Api.Actors;
using FancyEventStore.DapperProductionStore;
using FancyEventStore.EventStore.Abstractions;
using System.Diagnostics;

namespace FancyEventStore.DirectTests.Tests.Test2
{
    public class Test2ActorDapper : Test2Dapper
    {
        private readonly Anthill _anthill;

        public Test2ActorDapper(IEventStore eventStore, IDbContext context, Anthill anthill, string resultFileName, int retriesCount) : base(eventStore, context, resultFileName, retriesCount)
        {
            _anthill = anthill;
        }

        public override async Task Run()
        {
            await CleanData();
            await FillData();

            foreach (var @case in testCases)
            {
                var measurementId = Guid.NewGuid().ToString();
                var measurement = await _anthill.GetAnt<TemperatureMeasurementAnt>(measurementId);
                var temperature = Enumerable.Range(0, @case.Key)
                        .Select(x => (decimal)temperatureProvider.Next(-10, 40))
                        .ToList();

                await measurement.CreateAndRecord(temperature);

                _anthill.MarkUnused<TemperatureMeasurementAnt>(measurementId);

                for (int i = 0; i < retriesCount; i++)
                {
                    Console.WriteLine($"Case {@case.Key}; Attempt: {i}");

                    var sw = Stopwatch.StartNew();
                    measurement = await _anthill.GetAnt<TemperatureMeasurementAnt>(measurementId);    
                    sw.Stop();

                    testCases[@case.Key].Add(sw.ElapsedMilliseconds);
                }

                _anthill.MarkUnused<TemperatureMeasurementAnt>(measurementId);
            }

            File.Delete(resultFileName);
            foreach (var result in testCases)
            {
                File.AppendAllText(resultFileName, $"({result.Key}, {result.Value.Average().ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}), ");
            }
        }
    }
}
