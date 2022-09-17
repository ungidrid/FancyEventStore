using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.Common
{
    public static class TypeHelper
    {
        private static readonly ConcurrentDictionary<string, Type> _typeNamesCache = new();

        public static Type GetType(string typeName)
        {
            return _typeNamesCache.GetOrAdd(typeName, Type.GetType(typeName));
        }
    }
}
