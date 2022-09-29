using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.EventStoreDb.Helpers
{
    public class AggregateNotFoundException : Exception
    {
        public AggregateNotFoundException(string typeName, Guid id) : base($"{typeName} with id '{id}' was not found")
        {

        }

        public static AggregateNotFoundException For<T>(Guid id)
        {
            return new AggregateNotFoundException(typeof(T).Name, id);
        }
    }
}
