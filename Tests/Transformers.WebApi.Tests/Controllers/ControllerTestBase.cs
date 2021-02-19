using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Transformers.WebApi.Infrastructure.Mappings;

namespace Transformers.WebApi.Tests.Controllers
{
    public abstract class ControllerTestBase : EfTestBase
    {
        protected ControllerTestBase(Action<IServiceCollection> addServicesAction = null) : base(services =>
        {
            services.Scan(s => s.FromAssemblyOf<Startup>()
                .AddClasses(c => c.AssignableTo<ControllerBase>())
                .AsSelf()
                .WithTransientLifetime());
            services.AddAutoMapper(typeof(TransformerProfile));
            addServicesAction?.Invoke(services);
        })
        {
        }
    }
}
