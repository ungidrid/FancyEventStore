using FancyEventStore.Domain.Abstract;
using System.Collections.Concurrent;

using VersionSetterDelegateType = System.Action<object, object>;

namespace FancyEventStore.EventStore
{
    internal static class AggregateReflectionExtensions
    {
        private static readonly VersionSetterDelegateType _versionSetter;

        static AggregateReflectionExtensions()
        {
            _versionSetter = typeof(Aggregate)
                .GetProperty(nameof(Aggregate.Version))
                .SetValue;
        }

        public static void SetVersion<T>(this T aggregate, long version) where T : IAggregate
        {
            _versionSetter.Invoke(aggregate, version);
        }
    }
}
