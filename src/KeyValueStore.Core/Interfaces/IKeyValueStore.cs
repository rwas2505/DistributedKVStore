namespace KeyValueStore.Core.Interfaces
{
    public interface IKeyValueStore 
    {
        string? Get(string key);
        void Put(string key, string value);
        bool Delete(string key);
    }
}