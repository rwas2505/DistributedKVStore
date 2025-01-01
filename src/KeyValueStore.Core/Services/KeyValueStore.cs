using KeyValueStore.Core.Exceptions;
using KeyValueStore.Core.Interfaces;
using System.Collections.Concurrent;

namespace KeyValueStore.Core.Services
{
    public class KeyValueStore : IKeyValueStore
    {
        private readonly ConcurrentDictionary<string, string> _store = new();

        public string? Get(string key)
        {
            ValidateKey(key);

            if(_store.TryGetValue(key, out var value))
            {
                ValidateKey(key);

                return value;
            }

            return null;
        }

        public void Put(string key, string value)
        {
            ValidateKey(key);

            _store[key] = value;
        }

        public bool Delete(string key)
        {
            ValidateKey(key);

            return _store.TryRemove(key, out _);
        }

        //public void RestoreFromLog()

        private static void ValidateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidKeyException(ErrorMessages.InvalidKeyErrorMessage);
            }
        }
    }
}