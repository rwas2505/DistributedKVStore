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
        // Arrange & Act & Assert
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
        // Arrange & Act & Assert
        var exception = Assert.Throws<InvalidKeyException>(() => _keyValueStore.Delete(key));
        Assert.Equal(ErrorMessages.InvalidKeyErrorMessage, exception.Message);
    }

    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [Theory]
    public void PutThenGet_WhenValueIsNullEmptyOrWhitespace_ReturnsValue(string value)
    {
        // Arrange
        var key = "key";
        _keyValueStore.Put(key, value);

        // Act
        var actual = _keyValueStore.Get(key);

        // Assert
        Assert.True(actual.Exists);
        Assert.Equal(value, actual.Value);
    }

    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [Theory]
    public void Delete_WhenValueIsNullEmptyOrWhitespace_ReturnsValue(string value)
    {
        // Arrange
        var key = "key";
        _keyValueStore.Put(key, value);

        // Act
        var actual = _keyValueStore.Delete(key);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.Equal(value, actual.DeletedValue);
    }

    [Fact]
    public void Get_WhenKeyDoesNotExist_ShouldReturnFalseAndNull()
    {
        // Arrange
        var key = "non-existent key";
        _keyValueStore.Delete(key);

        // Act
        var actual = _keyValueStore.Get(key);

        // Assert
        Assert.False(actual.Exists);
        Assert.Null(actual.Value);
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
        Assert.True(actual.Exists);
        Assert.Equal(value, actual.Value);
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
        Assert.True(actual.Exists);
        Assert.Equal(valueSecond, actual.Value);
    }

    [Fact]
    public void Delete_WhenKeyDoesNotExist_ReturnsFalseAndNull()
    {
        // Arrange
        var key = "delete test 1";
        _keyValueStore.Delete(key); // Ensure key is deleted in case it exists

        // Act
        var actual = _keyValueStore.Delete(key);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.DeletedValue);
    }

    [Fact]
    public void Delete_WhenKeyDoesExist_ReturnsTrueAndValue()
    {
        // Arrange
        var key = "delete test 2";
        var value = "foobar";
        _keyValueStore.Put(key, value);

        // Act
        var actual = _keyValueStore.Delete(key);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.Equal(value, actual.DeletedValue);
    }

    [Fact]
    public void Get_ShouldRespectCaseSensitivityOfKeys()
    {
        // Arrange
        var keyLower = "testkey";
        var keyUpper = "TESTKEY";
        _keyValueStore.Put(keyLower, "value 1");
        _keyValueStore.Put(keyUpper, "value 2");

        // Act
        var valueLower = _keyValueStore.Get(keyLower);
        var valueUpper = _keyValueStore.Get(keyUpper);

        // Assert
        Assert.Equal("value 1", valueLower.Value);
        Assert.Equal("value 2", valueUpper.Value);
    }

    [Fact]
    public void PutThenGet_ConcurrentAccess_ShouldHandleSimultaneousOperationsCorrectly()
    {
        // Arrange
        var key = "concurrent test";
        var value = "value";

        // Act
        Parallel.For(0, 100, i =>
        {
            _keyValueStore.Put($"{key}-{i}", $"{value}-{i}");
        });

        // Assert
        Parallel.For(0, 100, i =>
        {
            var actual = _keyValueStore.Get($"{key}-{i}");
            Assert.Equal($"{value}-{i}", actual.Value);
        });

        Parallel.For(0, 100, i =>
        {
            var actual = _keyValueStore.Delete($"{key}-{i}");
            Assert.True(actual.IsSuccess);
            Assert.Equal($"{value}-{i}", actual.DeletedValue);
        });
    }
}