using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AntActor.Core
{
    public class Anthill
    {
        private readonly IAntResolver _antResolver;
        private static readonly ConcurrentDictionary<(Type, string), IAnt> _ants = new();

        public Anthill(IAntResolver antResolver)
        {
            _antResolver = antResolver;
        }

        public void RegisterAnt<T>(string id, T ant) where T: IAnt
        {
            _ants.AddOrUpdate((typeof(T), id), ant, (tuple, ant1) => ant1);
        }

        public async Task<T> GetAnt<T>(string id) where T: IAnt
        {
            var isNew = false;
            var instance = (T)_ants.GetOrAdd((typeof(T), id), _ => { isNew = true; return _antResolver.Resolve<T>(id); });

            if(isNew) await instance.OnActivateAsync();

            return instance;
        }

        public void MarkUnused<T>(string id)
        {
            _ants.TryRemove((typeof(T), id), out var actor);
            actor.Dispose();
        }
    }
}