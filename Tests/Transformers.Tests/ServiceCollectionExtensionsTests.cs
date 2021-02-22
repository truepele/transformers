using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Transformers.Model;
using Transformers.Model.Services;
using Transformers.Model.Services.War;
using Transformers.Model.Services.War.BattleRules;
using Xunit;

namespace Transformers.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddWarService_AddsBattleRules()
        {
            // Arrange
            var services = new ServiceCollection();
            services
                .Configure<WarSettings>(s => s.SpecialNames = System.Array.Empty<string>())
                .AddWarService();

            // Act
            var rules = services.BuildServiceProvider().GetRequiredService<IEnumerable<IBattleRule>>().ToList();

            // Assert
            Assert.True(rules[0] is SpecialNamesBattleRule);
            Assert.True(rules[1] is StrengthBattleRule);
            Assert.True(rules[2] is SkillBattleRule);
        }

        [Fact]
        public void AddWarService_AddsWarService()
        {
            // Arrange
            var services = new ServiceCollection();
            services
                .AddSingleton(Substitute.For<ITransformersDbContext>())
                .Configure<WarSettings>(s =>
                {
                    s.BattlePareIterator = new BattlePareIteratorSettings();
                    s.SpecialNames = System.Array.Empty<string>();
                })
                .AddWarService();

            // Act
            var service = services.BuildServiceProvider().GetRequiredService<IWarService>();

            // Assert
            Assert.True(service is WarService);
        }
    }
}
