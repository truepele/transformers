using System;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Transformers.DataAccess;
using Transformers.Model;
using Transformers.Model.Entities;
using Transformers.Model.Enums;

namespace Transformers.WebApi.Tests
{
    public abstract class EfTestBase : TestsBase, IDisposable
    {
        protected Faker<Transformer> TransformersFaker { get; }

        protected EfTestBase(Action<IServiceCollection> addServicesAction) : base(services =>
        {
            services.AddDbContext<ITransformersDbContext, TransformersDbContext>(o => o.UseInMemoryDatabase(nameof(EfTestBase)));
            addServicesAction(services);
        })
        {
            var transformersIds = 1;
            TransformersFaker = new Faker<Transformer>()
                .StrictMode(true)
                .RuleFor(o => o.Id, f => transformersIds++)
                .RuleFor(o => o.Name, f => f.Random.String(1, 25))
                .RuleFor(o => o.Allegiance, f => f.PickRandom(new[] { Allegiance.Autobot, Allegiance.Decepticon }))
                .RuleFor(o => o.Courage, f => f.Random.Number(1, 10))
                .RuleFor(o => o.Endurance, f => f.Random.Number(1, 10))
                .RuleFor(o => o.Firepower, f => f.Random.Number(1, 10))
                .RuleFor(o => o.Intelligence, f => f.Random.Number(1, 10))
                .RuleFor(o => o.Rank, f => f.Random.Number(1, 10))
                .RuleFor(o => o.Skill, f => f.Random.Number(1, 10))
                .RuleFor(o => o.Speed, f => f.Random.Number(1, 10))
                .RuleFor(o => o.Strength, f => f.Random.Number(1, 10));

        }

        protected async Task SeedDataAsync(Func<ITransformersDbContext, Task> seedActionAsync)
        {
            var dbContext = GetService<ITransformersDbContext>();
            await seedActionAsync(dbContext);
            await dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            var ctx = GetService<ITransformersDbContext>();
            ctx.Transformers.RemoveRange(ctx.Transformers.ToList());
            ctx.SaveChangesAsync().GetAwaiter().GetResult();
        }
    }
}
