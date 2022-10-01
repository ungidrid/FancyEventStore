using AntActor.Core;
using FancyEventStore.Api.Actors;
using FancyEventStore.DapperProductionStore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace FancyEventStore.DirectTests.Tests.Test5
{
    public class Test5ActorDapper : Test5Dapper
    {
        private readonly Anthill anthill;

        public Test5ActorDapper(IServiceProvider serviceProvider, IDbContext context, Anthill anthill, string resultFileName, int threadsCount) : base(serviceProvider, context, resultFileName, threadsCount)
        {
            this.anthill = anthill;
        }

        public override async Task Run()
        {
            await CleanData();
            await FillData();

            var actions = new Dictionary<int, (long TotalTime, int ErrorsCount)>();

            for (var parallelism = 1; parallelism <= threadsCount; parallelism++)
            {
                var tasks = new List<Task>();
                var time = new ConcurrentBag<long>();
                var errors = new ConcurrentBag<int>();
                string measurementId = Guid.NewGuid().ToString();

                TemperatureMeasurementAnt measurementAnt;
                using (var scope = serviceProvider.CreateAsyncScope())
                {
                    measurementAnt = await anthill.GetAnt<TemperatureMeasurementAnt>(measurementId);
                    await measurementAnt.StartMeasurement();
                }

                for (var t = 0; t < parallelism; t++)
                {
                    int taskNumber = t;
                    var task = Task.Run(async () =>
                    {
                        using var scope = serviceProvider.CreateScope();

                        int i = 1;

                        while (i <= actionsCount / parallelism)
                        {
                            Console.WriteLine($"Step {i}; Task {taskNumber}");
                            int errorsCount = 0;
                            var sw = Stopwatch.StartNew();

                            measurementAnt = await anthill.GetAnt<TemperatureMeasurementAnt>(measurementId);
                            await measurementAnt.Record(taskNumber);

                            sw.Stop();

                            time.Add(sw.ElapsedMilliseconds);
                            errors.Add(errorsCount);
                            i++;
                        }
                    });

                    tasks.Add(task);

                }

                await Task.WhenAll(tasks);
                anthill.MarkUnused<TemperatureMeasurementAnt>(measurementId);

                actions.Add(parallelism, (time.Sum(), errors.Sum()));
            }

            File.Delete(resultFileName);
            foreach (var result in actions)
            {
                File.AppendAllText(resultFileName, $"({result.Key}, {result.Value.TotalTime}), ");
            }
            File.AppendAllText(resultFileName, "\n");
            foreach (var result in actions)
            {
                File.AppendAllText(resultFileName, $"({result.Key}, {result.Value.ErrorsCount}), ");
            }
        }
    }
}
