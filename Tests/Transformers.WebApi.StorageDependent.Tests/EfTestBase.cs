using System;
using System.Threading.Tasks;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Transformers.DataAccess;
using Transformers.Model;
using Transformers.Model.Entities;
using Transformers.Model.Enums;
using Transformers.Tests;
using Transformers.WebApi.Infrastructure;
using Transformers.WebApi.Settings;

namespace Transformers.WebApi.StorageDependent.Tests
{
    public abstract class EfTestBase : TestsBase, IDisposable
    {
        protected Faker<Transformer> TransformersFaker { get; }

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

            TransformersFaker = new Faker<Transformer>()
                .StrictMode(true)
                .RuleFor(o => o.Id, _ => 0)
                .RuleFor(o => o.Name, f => Guid.NewGuid().ToString().Substring(0, Transformer.NameMaxLen))
                .RuleFor(o => o.Allegiance, f => f.PickRandom(new[] { Allegiance.Autobot, Allegiance.Decepticon }))
                .RuleFor(o => o.Courage, f => f.Random.Number(1, 10))
                .RuleFor(o => o.Endurance, f => f.Random.Number(1, 10))
                .RuleFor(o => o.Firepower, f => f.Random.Number(1, 10))
                .RuleFor(o => o.Intelligence, f => f.Random.Number(1, 10))
                .RuleFor(o => o.Rank, f => f.Random.Number(1, 10))
                .RuleFor(o => o.Skill, f => f.Random.Number(1, 10))
                .RuleFor(o => o.Speed, f => f.Random.Number(1, 10))
                .RuleFor(o => o.Strength, f => f.Random.Number(1, 10))
                .RuleFor(o => o.RowVersion, _ => null);

        }

        protected async Task SeedDataAsync(Func<ITransformersDbContext, Task> seedActionAsync)
        {
            var ctx = GetService<ITransformersDbContext>();
            await seedActionAsync(ctx);
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
