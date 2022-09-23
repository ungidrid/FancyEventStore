using AntActor.Core;
using FancyEventStore.Api.Actors;
using FancyEventStore.DapperProductionStore;
using FancyEventStore.EventStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                for (int i = 0; i < 10; i++)
                {
                    await CleanData();
                    await FillData();

                    Console.WriteLine($"Case {@case.Key}; Attempt: {i}");

                    var measurementAnt = _anthill.GetAnt<TemperatureMeasurementAnt>(Guid.NewGuid().ToString());
                    var startTask = measurementAnt.StartMeasurement();

                    var recordTasks = Enumerable.Range(0, @case.Key).Select(x => measurementAnt.Record(temperatureProvider.Next(-10, 40))).ToList();

                    await Task.WhenAll(recordTasks.Concat(new[] { startTask }));

                    var sw = Stopwatch.StartNew();
                    sw.Stop();

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
