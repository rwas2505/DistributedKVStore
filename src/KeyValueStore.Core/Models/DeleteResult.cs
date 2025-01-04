namespace KeyValueStore.Core.Models
{
    public record DeleteResult
    {
        public bool IsSuccess { get; init; }
        public string? DeletedValue { get; init; }
    }
}
