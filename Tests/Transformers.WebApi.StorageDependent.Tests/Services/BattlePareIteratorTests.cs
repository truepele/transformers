using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Transformers.Model.Entities;
using Transformers.Model.Enums;
using Transformers.Model.Services.War;
using Xunit;

namespace Transformers.WebApi.StorageDependent.Tests.Services
{
    [Collection(nameof(EfTestCollection))]
    public class BattlePareIteratorTests : EfTestBase
    {
        private readonly BattlePareIterator _sut;


        public BattlePareIteratorTests() : base(services =>
        {
            services.Configure<BattlePareIteratorSettings>(s => s.PageSize = 3);
            services.AddSingleton<BattlePareIterator>();
        })
        {
            _sut = GetService<BattlePareIterator>();
        }

        [Fact]
        public async Task Iterate_ReturnsExpected_MoreAutobots()
        {
            // Arrange
            var transformers = new List<Transformer>
            {
                TransformersFaker.RuleFor(t => t.Rank, 10)
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 9)
                    .RuleFor(t => t.Allegiance, Allegiance.Decepticon).Generate(),

                TransformersFaker.RuleFor(t => t.Rank, 9)
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 8)
                    .RuleFor(t => t.Allegiance, Allegiance.Decepticon).Generate(),

                TransformersFaker.RuleFor(t => t.Rank, 9)
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 7)
                    .RuleFor(t => t.Allegiance, Allegiance.Decepticon).Generate(),

                TransformersFaker.RuleFor(t => t.Rank, 5)
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 7)
                    .RuleFor(t => t.Allegiance, Allegiance.Decepticon).Generate(),

                TransformersFaker.RuleFor(t => t.Rank, 3)
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 1)
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 1)
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate()
            };
            await SeedDataAsync(ctx => ctx.Transformers.AddRangeAsync(transformers.OrderBy(t => t.Endurance)));

            // Act
            var result = await _sut.Iterate().ToListAsync();

            // Assert
            Assert.Equal((10, 9), (result[0].transformer1.Rank, result[0].transformer2.Rank));
            Assert.Equal((9, 8), (result[1].transformer1.Rank, result[1].transformer2.Rank));
            Assert.Equal((9, 7), (result[2].transformer1.Rank, result[2].transformer2.Rank));
            Assert.Equal((5, 7), (result[3].transformer1.Rank, result[3].transformer2.Rank));
            Assert.Equal((3, -1), (result[4].transformer1.Rank, result[4].transformer2?.Rank ?? -1));
            Assert.Equal((1, -1), (result[5].transformer1.Rank, result[5].transformer2?.Rank ?? -1));
            Assert.Equal((1, -1), (result[6].transformer1.Rank, result[6].transformer2?.Rank ?? -1));
        }

        [Fact]
        public async Task Iterate_ReturnsExpected_MoreDecepticons()
        {
            // Arrange
            var transformers = new List<Transformer>
            {
                TransformersFaker.RuleFor(t => t.Rank, 10)
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 9)
                    .RuleFor(t => t.Allegiance, Allegiance.Decepticon).Generate(),

                TransformersFaker.RuleFor(t => t.Rank, 9)
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 8)
                    .RuleFor(t => t.Allegiance, Allegiance.Decepticon).Generate(),

                TransformersFaker.RuleFor(t => t.Rank, 9)
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 7)
                    .RuleFor(t => t.Allegiance, Allegiance.Decepticon).Generate(),

                TransformersFaker.RuleFor(t => t.Rank, 5)
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 7)
                    .RuleFor(t => t.Allegiance, Allegiance.Decepticon).Generate(),

                TransformersFaker.RuleFor(t => t.Rank, 3)
                    .RuleFor(t => t.Allegiance, Allegiance.Decepticon).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 1)
                    .RuleFor(t => t.Allegiance, Allegiance.Decepticon).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 1)
                    .RuleFor(t => t.Allegiance, Allegiance.Decepticon).Generate()
            };
            await SeedDataAsync(ctx => ctx.Transformers.AddRangeAsync(transformers.OrderBy(t => t.Endurance)));

            // Act
            var result = await _sut.Iterate().ToListAsync();

            // Assert
            Assert.Equal((10, 9), (result[0].transformer1.Rank, result[0].transformer2.Rank));
            Assert.Equal((9, 8), (result[1].transformer1.Rank, result[1].transformer2.Rank));
            Assert.Equal((9, 7), (result[2].transformer1.Rank, result[2].transformer2.Rank));
            Assert.Equal((5, 7), (result[3].transformer1.Rank, result[3].transformer2.Rank));
            Assert.Equal((-1,3), (result[4].transformer1?.Rank ?? -1, result[4].transformer2.Rank));
            Assert.Equal((-1, 1), (result[5].transformer1?.Rank ?? -1, result[5].transformer2.Rank));
            Assert.Equal((-1, 1), (result[6].transformer1?.Rank ?? -1, result[6].transformer2.Rank));
        }

        [Fact]
        public async Task Iterate_ReturnsExpected_EqualNumberOfTransformers()
        {
            // Arrange
            var transformers = new List<Transformer>
            {
                TransformersFaker.RuleFor(t => t.Rank, 10)
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 9)
                    .RuleFor(t => t.Allegiance, Allegiance.Decepticon).Generate(),

                TransformersFaker.RuleFor(t => t.Rank, 9)
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 8)
                    .RuleFor(t => t.Allegiance, Allegiance.Decepticon).Generate(),

                TransformersFaker.RuleFor(t => t.Rank, 9)
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 7)
                    .RuleFor(t => t.Allegiance, Allegiance.Decepticon).Generate(),

                TransformersFaker.RuleFor(t => t.Rank, 5)
                    .RuleFor(t => t.Allegiance, Allegiance.Autobot).Generate(),
                TransformersFaker.RuleFor(t => t.Rank, 7)
                    .RuleFor(t => t.Allegiance, Allegiance.Decepticon).Generate()
            };
            await SeedDataAsync(ctx => ctx.Transformers.AddRangeAsync(transformers.OrderBy(t => t.Endurance)));

            // Act
            var result = await _sut.Iterate().ToListAsync();

            // Assert
            Assert.Equal((10, 9), (result[0].transformer1.Rank, result[0].transformer2.Rank));
            Assert.Equal((9, 8), (result[1].transformer1.Rank, result[1].transformer2.Rank));
            Assert.Equal((9, 7), (result[2].transformer1.Rank, result[2].transformer2.Rank));
            Assert.Equal((5, 7), (result[3].transformer1.Rank, result[3].transformer2.Rank));
        }
    }
}
