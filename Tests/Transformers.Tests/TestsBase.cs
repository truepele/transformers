using System;
using System.IO;
using Bogus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Transformers.Model.Entities;
using Transformers.Model.Enums;
using Transformers.Model.Services.War;

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
        protected Faker<Transformer> TransformersFaker { get; }

        protected TestsBase(Action<IServiceCollection> addServicesAction = null)
        {
            _services.Configure<WarSettings>(Configuration.GetSection("War"));
            addServicesAction?.Invoke(_services);
            _serviceProvider = _services.BuildServiceProvider();

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

        protected TService GetService<TService>()
        {
            return _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<TService>();
        }
    }
}
