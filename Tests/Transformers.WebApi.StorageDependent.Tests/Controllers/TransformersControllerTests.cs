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
            var result = await _sut.Get(allegiance).ToListAsync();

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

        [Fact]
        public async Task GetSingle_ReturnsExpected()
        {
            // Arrange
            var transformers = TransformersFaker.Generate(10);
            await SeedDataAsync(context => context.Transformers.AddRange(transformers));
            var transformer = transformers[3];

            // Act
            var result = await _sut.GetSingle(transformer.Id) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            ((TransformerDto)result.Value).ShouldDeepEqual(_mapper.Map<TransformerDto>(transformer));
        }

        [Fact]
        public async Task GetSingle_ReturnsNotFound()
        {
            // Arrange
            var transformers = TransformersFaker.Generate(10);
            await SeedDataAsync(context => context.Transformers.AddRange(transformers));

            // Act
            var result = await _sut.GetSingle(10000) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Update_ReturnsNotFound()
        {
            // Arrange
            var transformers = TransformersFaker.Generate(10);
            await SeedDataAsync(context => context.Transformers.AddRange(transformers));

            // Act
            var result = await _sut.Update(10000, new UpdateTransformerDto()) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Update_ReturnsConflict()
        {
            // Arrange
            var transformer = TransformersFaker.Generate();
            await SeedDataAsync(context => context.Transformers.Add(transformer));

            // Act
            var result = await _sut.Update(transformer.Id, new UpdateTransformerDto { RowVersion = "wrong" }) as ConflictResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Update_AppliesChanges()
        {
            // Arrange
            var transformer = TransformersFaker
                .RuleFor(t => t.Allegiance, Allegiance.Decepticon)
                .Generate();
            await SeedDataAsync(context => context.Transformers.Add(transformer));
            var dto = new UpdateTransformerDto
            {
                RowVersion = transformer.RowVersion.ToString(),
                Allegiance = Allegiance.Autobot,
                Courage = 1,
                Endurance = 2,
                Firepower = 3,
                Intelligence = 4,
                Name = "test name",
                Rank = 5,
                Skill = 6,
                Speed = 7,
                Strength = 8
            };

            // Act
            var result = await _sut.Update(transformer.Id, dto) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            await AssertResultsAsync(async context =>
            {
                var resultEntity = await context.Transformers.FindAsync(transformer.Id);
                Assert.Equal(dto.Allegiance, resultEntity.Allegiance);
                Assert.Equal(dto.Courage, resultEntity.Courage);
                Assert.Equal(dto.Endurance, resultEntity.Endurance);
                Assert.Equal(dto.Firepower, resultEntity.Firepower);
                Assert.Equal(dto.Intelligence, resultEntity.Intelligence);
                Assert.Equal(dto.Name, resultEntity.Name);
                Assert.Equal(dto.Rank, resultEntity.Rank);
                Assert.Equal(dto.Skill, resultEntity.Skill);
                Assert.Equal(dto.Speed, resultEntity.Speed);
                Assert.Equal(dto.Strength, resultEntity.Strength);
                result.Value.ShouldDeepEqual(_mapper.Map<TransformerDto>(resultEntity));
            });
        }

        [Fact]
        public async Task Delete_ReturnsNotFound()
        {
            // Arrange
            var transformers = TransformersFaker.Generate(10);
            await SeedDataAsync(context => context.Transformers.AddRange(transformers));

            // Act
            var result = await _sut.Delete(10000) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Delete_DeletesExpectedRecord()
        {
            // Arrange
            var transformers = TransformersFaker.Generate(10);
            var transformer = transformers[3];
            await SeedDataAsync(context => context.Transformers.AddRange(transformers));

            // Act
            var result = await _sut.Delete(transformer.Id) as OkResult;

            // Assert
            Assert.NotNull(result);
            await AssertResultsAsync(async context =>
            {
                var t = await context.Transformers.FindAsync(transformer.Id);
                Assert.Null(t);
                Assert.Equal(transformers.Count - 1, context.Transformers.Count());
            });
        }
    }
}
