using AntActor.Core;
using FancyEventStore.Api.Actors;
using FancyEventStore.DapperProductionStore;
using FancyEventStore.Domain.TemperatureMeasurement;
using FancyEventStore.EventStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.DirectTests.Tests.Test4
{
    public class Test4ActorDapper : Test4Dapper
    {
        private readonly Anthill _anthill;

        public Test4ActorDapper(IEventStore eventStore, IDbContext context, Anthill anthill, string resultFileName, int retriesCount) : base(eventStore, context, resultFileName, retriesCount)
        {
            _anthill = anthill;
        }

        public override async Task Run()
        {
            await CleanData();
            await FillData();

            var actions = new Dictionary<int, List<long>>();

            for (int j = 0; j < retriesCount; j++)
            {
                var measurementId = Guid.NewGuid().ToString();
                Console.WriteLine($"Try {j + 1}");

                for (int i = 1; i <= actionsCount; i++)
                {
                    Console.WriteLine($"Step {i + 1}");
                    var sw = Stopwatch.StartNew();
                    TemperatureMeasurementAnt measurementAnt = await _anthill.GetAnt<TemperatureMeasurementAnt>(measurementId);
                    if (i == 1)
                    {
                        await measurementAnt.StartMeasurement();
                    }
                    else
                    {
                        await measurementAnt.Record(temperatureProvider.Next(-20, 10));
                    }

                    sw.Stop();

                    if (i == 1 || i % recordEachAction == 0)
                    {
                        if (!actions.TryGetValue(i, out var _))
                        {
                            actions.Add(i, new List<long>());
                        }

                        actions[i].Add(sw.ElapsedMilliseconds);
                    }
                }
            }

            File.Delete(resultFileName);
            foreach (var result in actions)
            {
                File.AppendAllText(resultFileName, $"({result.Key}, {result.Value.Average().ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}), ");
            }
        }
    }
}
