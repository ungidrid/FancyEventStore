using FancyEventStore.Domain.TemperatureMeasurement;
using FancyEventStore.EventStore.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace FancyEventStore.DirectTests.Tests.Test5
{
    //Write to aggregate in parallel
    public abstract class Test5Base: TestBase
    {
        protected readonly IEventStore eventStore;
        protected readonly IServiceProvider serviceProvider;
        protected readonly string resultFileName;
        protected readonly int threadsCount;
        protected readonly int retriesCount;
        protected Random temperatureProvider = new(1);
        protected Random delayProvider = new(1);
        protected int actionsCount = 2000;

        public Test5Base(IServiceProvider serviceProvider, string resultFileName, int threadsCount)
        {
            this.serviceProvider = serviceProvider;
            this.resultFileName = resultFileName;
            this.threadsCount = threadsCount;
        }

        public override async Task Run()
        {
            await CleanData();
            await FillData();

            var actions = new Dictionary<int, (long TotalTime, int ErrorsCount)>();

            for (var parallelism = 1; parallelism <= threadsCount; parallelism++)
            {
                var threads = new List<Task>();
                var time = new ConcurrentBag<long>();
                var errors = new ConcurrentBag<int>();

                Guid measurementId;
                using (var scope = serviceProvider.CreateAsyncScope())
                {
                    var eventStore = scope.ServiceProvider.GetRequiredService<IEventStore>();
                    measurementId = Guid.NewGuid();
                    var measurement = TemperatureMeasurement.Start(measurementId);
                    await eventStore.Store(measurement);
                }

                for (var t = 0; t < parallelism; t++)
                {
                    int taskNumber = t;
                    var thread = Task.Run(async () =>
                    {
                        using var scope = serviceProvider.CreateScope();
                        var eventStore = scope.ServiceProvider.GetRequiredService<IEventStore>();

                        int i = 1;

                        while (i <= actionsCount / parallelism)
                        {
                            Console.WriteLine($"Step {i}; Task {taskNumber}");
                            int errorsCount = 0;
                            var sw = Stopwatch.StartNew();
                        X:
                            try
                            {
                                var measurement = await eventStore.Rehydrate<TemperatureMeasurement>(measurementId);
                                measurement.Record(taskNumber);
                                await eventStore.Store(measurement);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error {taskNumber}");
                                errorsCount++;
                                goto X;
                            }

                            sw.Stop();

                            time.Add(sw.ElapsedMilliseconds);
                            errors.Add(errorsCount);
                            i++;
                        }
                    });

                    threads.Add(thread);
                }

                await Task.WhenAll(threads);

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

        protected override async Task FillData()
        {
            using var scope = serviceProvider.CreateAsyncScope();
            var eventStore = scope.ServiceProvider.GetRequiredService<IEventStore>();

            const int initialStreamsCount = 100;
            var measurementsCountProvider = new Random(1);
            var temperatureProvider = new Random(1);

            for (int i = 0; i < initialStreamsCount; i++)
            {
                var temperatureMeasurement = TemperatureMeasurement.Start(Guid.NewGuid());

                var measurementsCount = measurementsCountProvider.Next(1, 20);
                for (int j = 0; j < measurementsCount; j++)
                {
                    temperatureMeasurement.Record(temperatureProvider.Next(-10, 40));
                }

                await eventStore.Store(temperatureMeasurement);
            }
        }
    }
}
