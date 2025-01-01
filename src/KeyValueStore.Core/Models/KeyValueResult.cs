public record KeyValueResult
{
    public bool Exists { get; init; }
    public string? Value { get; init; }
}