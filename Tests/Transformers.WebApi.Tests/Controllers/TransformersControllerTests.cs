using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DeepEqual.Syntax;
using Transformers.Model.Enums;
using Transformers.WebApi.Controllers;
using Transformers.WebApi.Dto;
using Xunit;

namespace Transformers.WebApi.Tests.Controllers
{
    public class TransformersControllerTests : ControllerTestBase
    {
        [Theory]
        [InlineData(Allegiance.Undefined)]
        [InlineData(Allegiance.Autobot)]
        [InlineData(Allegiance.Decepticon)]
        [InlineData(Allegiance.Undefined, 0)]
        [InlineData(Allegiance.Autobot, 0)]
        [InlineData(Allegiance.Decepticon, 0)]
        [InlineData(Allegiance.Undefined, 1)]
        [InlineData(Allegiance.Autobot, 1)]
        [InlineData(Allegiance.Decepticon, 1)]
        public async Task Get_ReturnsAll(Allegiance allegiance, int count = 20)
        {
            // Arrange

            var transformers = TransformersFaker.Generate(count).OrderBy(t => t.Name).ToList();
            var expectedTransformers = allegiance == Allegiance.Undefined
                ? transformers
                : transformers.Where(t => t.Allegiance == allegiance).ToList();
            var expectedDtoResult = GetService<IMapper>().Map<IEnumerable<TransformerDto>>(expectedTransformers);

            await SeedDataAsync(context => context.Transformers.AddRangeAsync(transformers));


            // Act
            var result = await GetService<TransformersController>().Get(allegiance);

            // Assert
            result.ShouldDeepEqual(expectedDtoResult);
        }
    }
}
