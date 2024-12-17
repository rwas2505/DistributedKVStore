namespace KeyValueStore.Core.Tests;
using KeyValueStore.Core.Services;
using KeyValueStore.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

public class KeyValueStoreTests : IClassFixture<TestFixture>
{
    private readonly IKeyValueStore _keyValueStore;

    public KeyValueStoreTests(TestFixture testFixture)
    {
        _keyValueStore = testFixture.ServiceProvider.GetRequiredService<IKeyValueStore>();
    }

    [Fact]
    public void PutAndGet_ShouldReturnCorrectValue()
    {
        // Arrange

        // Act
        _keyValueStore.Put("example", "test value");
        var result = _keyValueStore.Get("example");

        // Assert
        Assert.Equal("test value", result);
    }
}