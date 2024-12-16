using KeyValueStore.Core.Interfaces;
using System.Collections.Concurrent;

namespace KeyValueStore.Core.Services
{
    public class KeyValueStore : IKeyValueStore
    {
        private readonly ConcurrentDictionary<string, string> _store = new();

        public string? Get(string key)
        {
            _store.TryGetValue(key, out var value);
            return value;
        }

        public void Put(string key, string value)
        {
            _store[key] = value;
        }

        public bool Delete(string key)
        {
            return _store.TryRemove(key, out _);
        }

        //public void RestoreFromLog()
    }
}