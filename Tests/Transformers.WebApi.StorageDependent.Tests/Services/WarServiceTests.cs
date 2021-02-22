using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Transformers.Model;
using Transformers.Model.Entities;
using Transformers.Model.Enums;
using Transformers.Model.Services;
using Transformers.Model.Services.War;
using Xunit;

namespace Transformers.WebApi.StorageDependent.Tests.Services
{
    [Collection(nameof(EfTestCollection))]
    public class WarServiceTests : EfTestBase
    {
        private readonly IWarService _sut;


        public WarServiceTests() : base(services =>
        {
            services.AddWarService();

            var battle = Substitute.For<IBattleExecutor>();
            battle.ExecuteBattle(Arg.Any<Transformer>(), Arg.Any<Transformer>()).Returns(ci =>
            {
                var t1 = ci.ArgAt<Transformer>(0);
                var t2 = ci.ArgAt<Transformer>(1);
                if (t1 == null) { return t2; }

                if (t2 == null) { return t1; }

                if (t1.Courage > t2.Courage) { return t1; }

                if (t2.Courage > t1.Courage) { return t2; }

                return null;
            });
            services.AddSingleton(battle);
        })
        {
            _sut = GetService<IWarService>();
        }

        [Fact]
        public async Task Get_ReturnsExpected()
        {
            // Arrange

            var transformers = new List<Transformer>
            {
                TransformersFaker.RuleFor(t => t.Rank, 10).RuleFor(t => t.Courage, 5)
                    .RuleFor(t => t.Name, "Victor1")
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 9).RuleFor(t => t.Courage, 2)
                    .RuleFor(t => t.Name, "Looser1")
                    .RuleFor(t => t.Allegiance, Allegiance.Decepticon).Generate(),

                TransformersFaker.RuleFor(t => t.Rank, 9).RuleFor(t => t.Courage, 2)
                    .RuleFor(t => t.Name, "Looser2")
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 8).RuleFor(t => t.Courage, 5)
                .RuleFor(t => t.Name, "Victor2")
                .RuleFor(t => t.Allegiance, Allegiance.Decepticon).Generate(),

                TransformersFaker.RuleFor(t => t.Rank, 8).RuleFor(t => t.Courage, 2)
                    .RuleFor(t => t.Name, "Looser3")
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 7).RuleFor(t => t.Courage, 5)
                    .RuleFor(t => t.Name, "Victor3")
                    .RuleFor(t => t.Allegiance, Allegiance.Decepticon).Generate(),

                TransformersFaker.RuleFor(t => t.Rank, 7).RuleFor(t => t.Courage, 1)
                    .RuleFor(t => t.Name, "tie11")
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 6).RuleFor(t => t.Courage, 1)
                    .RuleFor(t => t.Name, "tie12")
                    .RuleFor(t => t.Allegiance, Allegiance.Decepticon).Generate(),

                TransformersFaker.RuleFor(t => t.Rank, 3).RuleFor(t => t.Courage, 1)
                .RuleFor(t => t.Name, "Victor4")
                .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 2).RuleFor(t => t.Courage, 1)
                    .RuleFor(t => t.Name, "Victor5")
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 1).RuleFor(t => t.Courage, 1)
                    .RuleFor(t => t.Name, "Victor6")
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate()
            };
            await SeedDataAsync(ctx => ctx.Transformers.AddRangeAsync(transformers.OrderBy(t => t.Endurance)));
            var victors = transformers.Where(t => t.Name.StartsWith("Victor")).OrderByDescending(t => t.Rank);

            // Act
            var result = await _sut.PerformWar().ToListAsync();

            // Assert
            result.ToList().ShouldDeepEqual(victors);
        }

        [Theory]
        [InlineData("Optimus", "Predaking")]
        [InlineData("Predaking", "Optimus")]
        public async Task Get_EndsWarNoVictors_WhenBothArmiesHaveSpecialNames(string autobotSpecialName, string decepticonSpecialName)
        {
            // Arrange
            var transformers = new List<Transformer>
            {
                TransformersFaker.RuleFor(t => t.Rank, 10).RuleFor(t => t.Courage, 5)
                    .RuleFor(t => t.Name, autobotSpecialName)
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 9).RuleFor(t => t.Courage, 2)
                    .RuleFor(t => t.Name, "Name1")
                    .RuleFor(t => t.Allegiance, Allegiance.Decepticon).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 9).RuleFor(t => t.Courage, 2)
                    .RuleFor(t => t.Name, "Name2")
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 8).RuleFor(t => t.Courage, 5)
                    .RuleFor(t => t.Name, decepticonSpecialName)
                    .RuleFor(t => t.Allegiance, Allegiance.Decepticon).Generate(),
            };
            await SeedDataAsync(ctx => ctx.Transformers.AddRangeAsync(transformers.OrderBy(t => t.Endurance)));

            // Act
            var result = await _sut.PerformWar().ToListAsync();

            // Assert
            Assert.Empty(result);
        }
    }
}
