using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Transformers.DataAccess.Services;
using Transformers.Model;
using Transformers.Model.Entities;
using Xunit;

namespace Transformers.WebApi.StorageDependent.Tests.Services
{
    public class OverallScoreCalcServiceStoredProcTests : EfTestBase
    {
        private readonly OverallRatingCalcServiceStoredProc _sut;

        public OverallScoreCalcServiceStoredProcTests() : base(services =>
        {
            services.AddSingleton<OverallRatingCalcServiceStoredProc>()
                .AddSingleton<Func<IDbConnection>>(p => () =>
                    (p.GetRequiredService<ITransformersDbContext>() as DbContext).Database.GetDbConnection());
        })
        {
            _sut = GetService<OverallRatingCalcServiceStoredProc>();
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

            // Act
            var result = await _sut.CalculateAsync(transformer);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
