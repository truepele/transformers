using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DeepEqual.Syntax;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Transformers.DataAccess.Services;
using Transformers.Model;
using Transformers.Model.Entities;
using Transformers.Model.Enums;
using Transformers.Model.Services;
using Transformers.WebApi.Controllers;
using Transformers.WebApi.Dto;
using Xunit;

namespace Transformers.WebApi.StorageDependent.Tests.Controllers
{
    public class TransformersControllerTests : EfControllerTestBase
    {
        private readonly TransformersController _sut;
        private readonly IMapper _mapper;

        public TransformersControllerTests(): base(services =>
        {
            services.AddSingleton<IOverallScoreCalcService, OverallScoreCalcServiceStoredProc>()
                .AddSingleton<Func<IDbConnection>>(p => () =>
                    (p.GetRequiredService<ITransformersDbContext>() as DbContext).Database.GetDbConnection());
        })
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

        [Theory]
        [InlineData(0,0,0,0,0,0,0,0,0)]
        [InlineData(1,1,1,1,1,1,1,2,9)]
        [InlineData(10,0,9,0,8,0,7,0,34)]
        [InlineData(10,10,10,10,10,10,10,10,80)]
        public async Task GetOverallScore_ReturnsExpected(
            int courage,
            int endurance,
            int firepower,
            int intelligence,
            int skill,
            int speed,
            int strength,
            int rank,
            int expectedResult)
        {
            // Arrange
            var transformer = new Transformer
            {
                Courage = courage,
                Endurance = endurance,
                Firepower = firepower,
                Intelligence = intelligence,
                Skill = skill,
                Speed = speed,
                Strength = strength,
                Rank = rank
            };
            await SeedDataAsync(context => context.Transformers.Add(transformer));

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
