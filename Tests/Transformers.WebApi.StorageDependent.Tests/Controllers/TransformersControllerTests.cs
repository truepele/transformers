using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DeepEqual.Syntax;
using Microsoft.EntityFrameworkCore;
using Transformers.Model.Entities;
using Transformers.Model.Enums;
using Transformers.WebApi.Controllers;
using Transformers.WebApi.Dto;
using Xunit;

namespace Transformers.WebApi.StorageDependent.Tests.Controllers
{
    public class TransformersControllerTests : EfControllerTestBase
    {
        private readonly TransformersController _sut;
        private readonly IMapper _mapper;

        public TransformersControllerTests()
        {
            _sut = GetService<TransformersController>();
            _mapper = GetService<IMapper>();
        }

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

            await SeedDataAsync(context => context.Transformers.AddRangeAsync(transformers));
            var expectedDtoResult = _mapper.Map<IEnumerable<TransformerDto>>(expectedTransformers);


            // Act
            var result = await _sut.Get(allegiance);

            // Assert
            result.ShouldDeepEqual(expectedDtoResult);
        }

        [Theory]
        [InlineData(Allegiance.Undefined)]
        [InlineData(Allegiance.Autobot)]
        [InlineData(Allegiance.Decepticon)]
        public async Task Create_CreatesSuccessfully(Allegiance allegiance)
        {
            // Arrange
            var newTransformerDto = new NewTransformerDto
            {
                Allegiance = allegiance,
                Courage = 1,
                Endurance = 2,
                Firepower = 3,
                Intelligence = 4,
                Name = Guid.NewGuid().ToString().Substring(0, Transformer.NameMaxLen),
                Rank = 5,
                Skill = 6,
                Speed = 7,
                Strength = 8
            };

            // Act
            var resultDto = await GetService<TransformersController>().Create(newTransformerDto);

            // Assert
            await AssertResultsAsync(async context =>
            {
                var result = await context.Transformers.SingleAsync();
                Assert.Equal(newTransformerDto.Allegiance, result.Allegiance);
                Assert.Equal(newTransformerDto.Courage, result.Courage);
                Assert.Equal(newTransformerDto.Endurance, result.Endurance);
                Assert.Equal(newTransformerDto.Firepower, result.Firepower);
                Assert.Equal(newTransformerDto.Intelligence, result.Intelligence);
                Assert.Equal(newTransformerDto.Name, result.Name);
                Assert.Equal(newTransformerDto.Rank, result.Rank);
                Assert.Equal(newTransformerDto.Skill, result.Skill);
                Assert.Equal(newTransformerDto.Strength, result.Strength);
                resultDto.ShouldDeepEqual(_mapper.Map<TransformerDto>(result));
            });
        }
    }
}
