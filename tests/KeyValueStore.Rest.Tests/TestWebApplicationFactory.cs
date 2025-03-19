using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace KeyValueStore.Rest.Tests;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly Dictionary<Type, object> _mockedServices = new();

    public void SetupService<TService>(Mock<TService> mockedService) where TService : class
    {
        _mockedServices[typeof(TService)] = mockedService;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace real services with mocks
            foreach (var mockedService in _mockedServices)
            {
                // Remove the existing service registration if it exists
                var descriptor = services.SingleOrDefault(d => d.ServiceType == mockedService.Key);
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add the mocked service
                if (mockedService.Value is Mock mockObject)
                {
                    services.AddSingleton(mockedService.Key, mockObject.Object);
                }
            }
        });

        return base.CreateHost(builder);
    }
}