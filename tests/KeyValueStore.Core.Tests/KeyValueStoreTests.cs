namespace KeyValueStore.Core.Tests;
using KeyValueStore.Core.Services;

public class KeyValueStoreTests
{
    [Fact]
    public void PutAndGet_ShouldReturnCorrectValue()
    {
        // Arrange
        var store = new KeyValueStore();

        // Act
        store.Put("example", "test value");
        var result = store.Get("example");

        // Assert
        Assert.Equal("test value", result);
    }
}