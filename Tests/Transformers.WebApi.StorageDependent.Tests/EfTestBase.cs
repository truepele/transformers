using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Transformers.DataAccess;
using Transformers.Model;
using Transformers.Tests;
using Transformers.WebApi.Infrastructure;
using Transformers.WebApi.Settings;
using Xunit;

namespace Transformers.WebApi.StorageDependent.Tests
{
    public abstract class EfTestBase: TestsBase, IDisposable, IClassFixture<EfTestFixture>
    {
        protected EfTestBase(Action<IServiceCollection> addServicesAction) : base(services =>
        {
            services.Configure<DataAccessSettings>(Configuration.GetSection("DataAccess"));
            services.AddDbContext();
            addServicesAction(services);
        })
        {
            var ctx = GetService<ITransformersDbContext>() as TransformersDbContext;
            if (GetService<IOptions<DataAccessSettings>>().Value.DataAccessType != DataAccessType.InMemory)
            {
                ctx.Database.Migrate();
            }
            ctx.Transformers.RemoveRange(ctx.Transformers);
            ctx.SaveChanges();
        }

        protected async Task SeedDataAsync(Func<ITransformersDbContext, Task> seedActionAsync)
        {
            var ctx = GetService<ITransformersDbContext>();
            await seedActionAsync(ctx);
            await ctx.SaveChangesAsync();
        }

        protected async Task SeedDataAsync(Action<ITransformersDbContext> seedAction)
        {
            var ctx = GetService<ITransformersDbContext>();
            seedAction(ctx);
            await ctx.SaveChangesAsync();
        }

        protected async Task AssertResultsAsync(Func<ITransformersDbContext, Task> assertActionAsync)
        {
            await assertActionAsync(GetService<ITransformersDbContext>());
        }

        public void Dispose()
        {
            var ctx = GetService<ITransformersDbContext>() as TransformersDbContext;
            ctx.Transformers.RemoveRange(ctx.Transformers);
            ctx.SaveChanges();
        }
    }
}
