using KeyValueStore.Core.Models;

namespace KeyValueStore.Core.Interfaces
{
    public interface IKeyValueStore 
    {
        GetResult Get(string key);
        PutResult Put(string key, string value);
        DeleteResult Delete(string key);
    }
}