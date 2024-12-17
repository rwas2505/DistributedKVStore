using Microsoft.Extensions.DependencyInjection;
using KeyValueStore.Core.Services;
using KeyValueStore.Core.Interfaces;

namespace KeyValueStore.Core.Tests
{
    public class TestFixture
    {
        public ServiceProvider ServiceProvider { get; private set; }

        public TestFixture()
        {
            var services = new ServiceCollection();

            // Register services
            services.AddSingleton<IKeyValueStore, Services.KeyValueStore>();
            
            // Build the service provider
            ServiceProvider = services.BuildServiceProvider();
        }
    }
}