public record GetResult
{
    public bool Exists { get; init; }
    public string? Value { get; init; }
}