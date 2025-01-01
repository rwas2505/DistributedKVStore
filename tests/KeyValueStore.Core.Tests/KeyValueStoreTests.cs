namespace KeyValueStore.Core.Tests;
using KeyValueStore.Core.Services;
using KeyValueStore.Core.Interfaces;
using KeyValueStore.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

public class KeyValueStoreTests : IClassFixture<TestFixture>
{
    private readonly IKeyValueStore _keyValueStore;

    public KeyValueStoreTests(TestFixture testFixture)
    {
        _keyValueStore = testFixture.ServiceProvider.GetRequiredService<IKeyValueStore>();
    }

    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [Theory]
    public void Get_WhenKeyIsNullEmptyOrWhitespace_ThrowsInvalidKeyException(string key)
    {
        // Arrange& Act & Assert
        var exception = Assert.Throws<InvalidKeyException>(() => _keyValueStore.Get(key));
        Assert.Equal(ErrorMessages.InvalidKeyErrorMessage, exception.Message);
    }

    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [Theory]
    public void Put_WhenKeyIsNullEmptyOrWhitespace_ThrowsInvalidKeyException(string key)
    {
        // Arrange
        var value = "value";

        // Act & Assert
        var exception = Assert.Throws<InvalidKeyException>(() => _keyValueStore.Put(key, value));
        Assert.Equal(ErrorMessages.InvalidKeyErrorMessage, exception.Message);
    }

    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [Theory]
    public void Delete_WhenKeyIsNullEmptyOrWhitespace_ThrowsInvalidKeyException(string key)
    {
        // Arrange& Act & Assert
        var exception = Assert.Throws<InvalidKeyException>(() => _keyValueStore.Delete(key));
        Assert.Equal(ErrorMessages.InvalidKeyErrorMessage, exception.Message);
    }

    [Fact]
    public void PutOnceAndGet_ShouldReturnCorrectValue()
    {
        // Arrange
        var key = "test key";
        var value = "test value";

        // Act
        _keyValueStore.Put(key, value);
        var actual = _keyValueStore.Get(key);

        // Assert
        Assert.Equal(value, actual);
    }

    [Fact]
    public void PutTwiceAndGet_ShouldReturnLatestValue()
    {
        // Arrange
        var key = "test key";
        var valueFirst = "first";
        var valueSecond = "second";

        // Act
        _keyValueStore.Put(key, valueFirst);
        _keyValueStore.Put(key, valueSecond);
        var actual = _keyValueStore.Get(key);

        // Assert
        Assert.Equal(valueSecond, actual);
    }

    [Fact]
    public void Delete_WhenKeyDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var key = "delete test 1";
        _keyValueStore.Delete(key); // Ensure key is deleted in case it exists

        // Act
        var actual = _keyValueStore.Delete(key);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void Delete_WhenKeyDoesExist_ReturnsTrue()
    {
        // Arrange
        var key = "delete test 2";
        var value = "foobar";
        _keyValueStore.Put(key, value);

        // Act
        var actual = _keyValueStore.Delete(key);
        Assert.True(actual);
    }
}