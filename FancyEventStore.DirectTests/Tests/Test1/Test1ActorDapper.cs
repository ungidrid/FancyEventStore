using AntActor.Core;
using FancyEventStore.Api.Actors;
using FancyEventStore.DapperProductionStore;
using FancyEventStore.EventStore.Abstractions;
using System.Diagnostics;

namespace FancyEventStore.DirectTests.Tests.Test1
{
    internal class Test1ActorDapper : Test1Dapper
    {
        private readonly Anthill _anthill;

        public Test1ActorDapper(IEventStore eventStore, Anthill anthill, IDbContext context, string resultFileName, int retriesCount) : base(eventStore, context, resultFileName, retriesCount)
        {
            this._anthill = anthill;
        }

        public override async Task Run()
        {
            foreach (var @case in testCases)
            {
                for (int i = 0; i < retriesCount; i++)
                {
                    await CleanData();
                    await FillData();

                    Console.WriteLine($"Case {@case.Key}; Attempt: {i}");

                    var id = Guid.NewGuid().ToString();
                    var measurementAnt = await _anthill.GetAnt<TemperatureMeasurementAnt>(id);
                    var temperature = Enumerable.Range(0, @case.Key)
                        .Select(x => (decimal)temperatureProvider.Next(-10, 40))
                        .ToList();

                    var sw = Stopwatch.StartNew();
                    await measurementAnt.CreateAndRecord(temperature);
                    sw.Stop();

                    _anthill.MarkUnused<TemperatureMeasurementAnt>(id);

                    testCases[@case.Key].Add(sw.ElapsedMilliseconds);
                }
            }

            foreach (var result in testCases)
            {
                File.AppendAllText(resultFileName, $"({result.Key}, {result.Value.Average().ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}), ");
            }
        }
    }
}
