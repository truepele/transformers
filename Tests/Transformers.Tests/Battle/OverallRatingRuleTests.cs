using System;
using Transformers.Model.Entities;
using Transformers.Model.Services.War.BattleRules;
using Xunit;

namespace Transformers.Tests.Battle
{
    public class OverallRatingRuleTests
    {
        // Rule: the victor is whomever of Transformer A and B has the higher overall rating

        [Fact]
        public void CheckIsVictor_ThrowsArgumentNullException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(() => new OverallRatingBattleRule().CheckIsVictor(null, new Transformer()));
            Assert.Throws<ArgumentNullException>(() => new OverallRatingBattleRule().CheckIsVictor(new Transformer(), null));
            Assert.Throws<ArgumentNullException>(() => new OverallRatingBattleRule().CheckIsVictor(null, null));
        }

        [Theory]
        [InlineData(10, 1, true)]
        [InlineData(5, 4, true)]
        [InlineData(5, 5, false)]
        [InlineData(4, 5, false)]
        [InlineData(1, 10, false)]
        public void CheckIsVictor_ReturnsExpected(int overallRating1, int overallRating2, bool expectedResult)
        {
            // Arrange
            var t1 = new Transformer { OverallRating = overallRating1 };
            var t2 = new Transformer { OverallRating = overallRating2 };
            var sut = new OverallRatingBattleRule();

            // Act
            var result = sut.CheckIsVictor(t1, t2);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
