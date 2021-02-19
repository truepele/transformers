using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Transformers.WebApi.Infrastructure.Mappings;

namespace Transformers.WebApi.StorageDependent.Tests.Controllers
{
    public abstract class EfControllerTestBase : EfTestBase
    {
        protected EfControllerTestBase(Action<IServiceCollection> addServicesAction = null) : base(services =>
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
