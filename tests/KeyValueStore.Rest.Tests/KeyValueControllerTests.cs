using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using KeyValueStore.Core.Interfaces;
using KeyValueStore.Rest.Controllers;
using KeyValueStore.Rest.Models;

namespace KeyValueStore.Rest.Tests;

public class KeyValueControllerTests
{
    private readonly Mock<IKeyValueStore> _mockStore;
    private readonly StoreController _controller;

    public KeyValueControllerTests()
    {
        _mockStore = new Mock<IKeyValueStore>();
        _controller = new StoreController(_mockStore.Object);
    }

        // Test for Get method: Should return 200 and the correct value
        [Fact]
        public void Get_ReturnsOkResult_WithExpectedValue(){

        }
}