using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DeepEqual.Syntax;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Transformers.Model.Entities;
using Transformers.Model.Enums;
using Transformers.Model.Services;
using Transformers.WebApi.Controllers;
using Transformers.WebApi.Dto;
using Xunit;

namespace Transformers.WebApi.StorageDependent.Tests.Controllers
{
    [Collection(nameof(EfTestCollection))]
    public class TransformersControllerTests : EfControllerTestBase
    {
        private readonly TransformersController _sut;
        private readonly IMapper _mapper;
        private readonly IOverallRatingCalcService _overallScoreCalcStub;

        public TransformersControllerTests(): base(services =>
        {
            services.AddSingleton(Substitute.For<IOverallRatingCalcService>());
        })
        {
            _sut = GetService<TransformersController>();
            _overallScoreCalcStub = GetService<IOverallRatingCalcService>();
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
        public async Task Get_ReturnsExpected(Allegiance allegiance, int count = 20)
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
        [InlineData(Allegiance.Autobot)]
        [InlineData(Allegiance.Decepticon)]
        public async Task Create_CreatesSuccessfully(Allegiance allegiance)
        {
            // Arrange
            const int expectedOverallRate = 123;
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
            _overallScoreCalcStub.CalculateAsync(Arg.Is<Transformer>(t => t.Name == newTransformerDto.Name))
                .Returns(expectedOverallRate);

            // Act
            var resultDto = await GetService<TransformersController>().Create(newTransformerDto);

            // Assert
            await AssertResultsAsync(async context =>
            {
                var result = await context.Transformers.AsQueryable().SingleAsync();
                Assert.Equal(expectedOverallRate, result.OverallRating);
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

        [Fact]
        public async Task GetOverallScore_ReturnsExpected()
        {
            // Arrange
            const int expectedResult = 123;
            var transformer = TransformersFaker.Generate();
            await SeedDataAsync(context => context.Transformers.Add(transformer));
            _overallScoreCalcStub.CalculateAsync(Arg.Is<Transformer>(t => t.Id == transformer.Id))
                .Returns(expectedResult);

            // Act
            var result = await _sut.GetOverallScore(transformer.Id) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult, (int)result.Value);
        }

        [Fact]
        public async Task GetOverallScore_ReturnsNotFound()
        {
            // Arrange
            var transformer = new Transformer();
            await SeedDataAsync(context => context.Transformers.Add(transformer));

            // Act
            var result = await _sut.GetOverallScore(transformer.Id + 1) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
        }
    }
}
