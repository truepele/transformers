using System;
using Microsoft.Extensions.DependencyInjection;

namespace Transformers.WebApi.Tests
{
    public abstract class TestsBase
    {
        private readonly IServiceCollection _services = new ServiceCollection();
        private readonly ServiceProvider _serviceProvider;

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
