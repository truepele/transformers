using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Transformers.Tests
{
    public abstract class TestsBase
    {
        private readonly IServiceCollection _services = new ServiceCollection();
        private readonly ServiceProvider _serviceProvider;
        private static readonly Lazy<IConfiguration> _configurationRootLazy = new(() =>
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json", true)
                .AddEnvironmentVariables();
            return configBuilder.Build();
        });

        public static IConfiguration Configuration => _configurationRootLazy.Value;

        protected TestsBase(Action<IServiceCollection> addServicesAction)
        {
            addServicesAction(_services);
            _serviceProvider = _services.BuildServiceProvider();
        }

        protected TService GetService<TService>()
        {
            return _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<TService>();
        }
    }
}
