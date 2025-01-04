using KeyValueStore.Core.Exceptions;
using KeyValueStore.Core.Interfaces;
using KeyValueStore.Core.Models;
using System.Collections.Concurrent;

namespace KeyValueStore.Core.Services
{
    public class KeyValueStore : IKeyValueStore
    {
        private readonly ConcurrentDictionary<string, string> _store = new();

        public GetResult Get(string key)
        {
            ValidateKey(key);

            if (_store.TryGetValue(key, out var value))
            {
                return new GetResult { Exists = true, Value = value };
            }

            return new GetResult { Exists = false };
        }

        public PutResult Put(string key, string? value)
        {
            ValidateKey(key);

            var isUpdate = _store.ContainsKey(key);
            _store[key] = value;

            return new PutResult
            {
                IsSuccess = true,
                IsUpdate = isUpdate
            };
        }

        public DeleteResult Delete(string key)
        {
            ValidateKey(key);

            if (_store.TryRemove(key, out var deletedValue))
            {
                return new DeleteResult
                {
                    IsSuccess = true,
                    DeletedValue = deletedValue
                };
            }

            return new DeleteResult { IsSuccess = false };
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