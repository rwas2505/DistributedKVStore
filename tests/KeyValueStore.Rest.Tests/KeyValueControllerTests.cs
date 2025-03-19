using Moq;
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

    public KeyValueControllerTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_KeyExists_ReturnsOkResultWithKeyValue()
    {
        // Arrange
        var key = "test-key-get-exists";
        var value = "test-value-get-exists";
        var storeMock = new Mock<IKeyValueStore>();
        storeMock.Setup(s => s.Get(key)).Returns(new GetResult { Value = value });
        
        var client = CreateClientWithMock(storeMock);

        // Act
        var response = await client.GetAsync($"store/{key}");
        var result = await response.Content.ReadFromJsonAsync<GetResult>();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(result);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public async Task Get_KeyDoesNotExist_ReturnsOkResultWithNullValue()
    {
        // Arrange
        var key = "test-key-get-not-exists";
        var storeMock = new Mock<IKeyValueStore>();
        storeMock.Setup(s => s.Get(key)).Returns(new GetResult { Value = null });
        
        var client = CreateClientWithMock(storeMock);

        // Act
        var response = await client.GetAsync($"store/{key}");
        var result = await response.Content.ReadFromJsonAsync<GetResult>();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(result);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task Put_KeyAndValueProvided_IsNotUpdate_ReturnsSuccessAndNotUpdate()
    {
        // Arrange
        var key = "test-key-put";
        var request = new PutRequestDto("test-value-put");
        var storeMock = new Mock<IKeyValueStore>();
        storeMock.Setup(s => s.Put(key, request.Value)).Returns(new PutResult { IsSuccess = true, IsUpdate = false });
        
        var client = CreateClientWithMock(storeMock);

        // Act
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"store/{key}", content);
        var result = await response.Content.ReadFromJsonAsync<PutResult>();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(result);
        Assert.False(result.IsUpdate);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_KeyProvided_ReturnsOkResult()
    {
        // Arrange
        var key = "test-key-delete";
        var value = "test-value-delete";
        var storeMock = new Mock<IKeyValueStore>();
        storeMock.Setup(s => s.Delete(key)).Returns(new DeleteResult { IsSuccess = true, DeletedValue = value });
        
        var client = CreateClientWithMock(storeMock);

        // Act
        var response = await client.DeleteAsync($"store/{key}");
        var result = await response.Content.ReadFromJsonAsync<DeleteResult>();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.DeletedValue);
    }

    private HttpClient CreateClientWithMock(Mock<IKeyValueStore> storeMock)
    {
        // Create a fresh factory instance for each test
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace the IKeyValueStore with the mock for this specific test
                _factory.SetupService(storeMock);
            });
        });
        
        return factory.CreateClient();
    }
}