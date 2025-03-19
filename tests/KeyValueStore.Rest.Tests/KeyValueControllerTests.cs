using Moq;
using Xunit;
using KeyValueStore.Core.Interfaces;
using KeyValueStore.Rest.Models;
using System.Text;
using System.Net.Http.Json;
using KeyValueStore.Core.Models;
using System.Text.Json;

namespace KeyValueStore.Rest.Tests;

public class KeyValueControllerTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly Mock<IKeyValueStore> _storeMock;
    private readonly HttpClient _client;

    public KeyValueControllerTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _storeMock = new Mock<IKeyValueStore>();
        _factory.SetupService(_storeMock);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async void Get_KeyExists_ReturnsOkResultWithKeyValue()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";
        _storeMock.Setup(s => s.Get(key)).Returns(new GetResult { Value = value });

        // Act
        var response = await _client.GetAsync($"store/{key}");
        var result = await response.Content.ReadFromJsonAsync<GetResult>();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(result);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public async void Get_KeyDoesNotExist_ReturnsOkResultWithNullValue()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();
        _storeMock.Setup(s => s.Get(key)).Returns(new GetResult{ Value = null });

        // Act
        var response = await _client.GetAsync($"store/{key}");
        var result = await response.Content.ReadFromJsonAsync<GetResult>();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(result);
        Assert.Null(result.Value);
    }

    [Fact]
    public async void Put_KeyAndValueProvided_IsNotUpdate_ReturnsSuccessAndNotUpdate()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();
        var request = new PutRequestDto("test-value");
        _storeMock.Setup(s => s.Put(key, request.Value)).Returns(new PutResult{ IsSuccess = true, IsUpdate = false });

        // Act
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"store/{key}", content);
        var result = await response.Content.ReadFromJsonAsync<PutResult>();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(result);
        Assert.False(result.IsUpdate);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async void Delete_KeyProvided_ReturnsOkResult()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();
        var value = "somevalue";
        _storeMock.Setup(s => s.Delete(key)).Returns(new DeleteResult { IsSuccess = true, DeletedValue = value});

        // Act
        var response = await _client.DeleteAsync($"store/{key}");
        var result = await response.Content.ReadFromJsonAsync<DeleteResult>();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.DeletedValue);
    }
}