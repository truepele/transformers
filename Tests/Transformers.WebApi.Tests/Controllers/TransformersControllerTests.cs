using System;
using System.Threading.Tasks;
using Bogus;
using DeepEqual.Syntax;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Transformers.DataAccess;
using Transformers.Model;
using Transformers.Model.Entities;
using Transformers.Model.Enums;
using Transformers.WebApi.Controllers;
using Xunit;

namespace Transformers.WebApi.Tests.Controllers
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

    public abstract class EfTestBase : TestsBase
    {
        private int _transformerIds = 0;
        public Faker<Transformer> TransformersFaker { get; }

        protected EfTestBase(Action<IServiceCollection> addServicesAction) : base(services =>
        {
            services.AddDbContext<ITransformersDbContext, TransformersDbContext>(o => o.UseInMemoryDatabase(nameof(EfTestBase)));
            addServicesAction(services);
        })
        {
            TransformersFaker = new Faker<Transformer>()
                .StrictMode(true)
                .RuleFor(o => o.Id, f => 0)
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

        protected async Task AssertResultsAsync(Func<ITransformersDbContext, Task> assertActionAsync)
        {
            await assertActionAsync(GetService<ITransformersDbContext>());
        }
    }

    public abstract class ControllerTestBase : EfTestBase
    {
        protected ControllerTestBase(Action<IServiceCollection> addServicesAction = null) : base(services =>
        {
            services.Scan(s => s.FromAssemblyOf<Startup>()
                .AddClasses(c => c.AssignableTo<ControllerBase>())
                .AsSelf()
                .WithTransientLifetime());
            addServicesAction?.Invoke(services);
        })
        {
        }
    }

    public class TransformersControllerTests : ControllerTestBase
    {
        [Fact]
        public async Task Get_ReturnsAll()
        {
            // Arrange
            var transformers = TransformersFaker.Generate(50);
            await SeedDataAsync(context => context.Transformers.AddRangeAsync(transformers));

            // Act
            var result = await GetService<TransformersController>().GetAll();

            // Assert
            result.ShouldDeepEqual(transformers);
        }
    }
}
