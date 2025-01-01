namespace KeyValueStore.Core.Models
{
    public record PutResult
    {
        public bool IsSuccess { get; init; }
        public bool IsUpdate { get; init; }
    }
}
