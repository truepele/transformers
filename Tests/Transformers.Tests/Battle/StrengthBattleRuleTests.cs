using System;
using Transformers.Model.Entities;
using Transformers.Model.Services.War.BattleRules;
using Xunit;

namespace Transformers.Tests.Battle
{
    public class StrengthBattleRuleTests
    {
        // Rule: If Transformer A exceeds Transformer B in strength by 3 or more and Transformer B has less than 5 courage, the battle is won by Transformer A


        [Fact]
        public void CheckIsVictor_ThrowsArgumentNullException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(() => new StrengthBattleRule().CheckIsVictor(null, new Transformer()));
            Assert.Throws<ArgumentNullException>(() => new StrengthBattleRule().CheckIsVictor(new Transformer(), null));
            Assert.Throws<ArgumentNullException>(() => new StrengthBattleRule().CheckIsVictor(null, null));
        }

        [Theory]
        [InlineData(10, 7, 4, true)]
        [InlineData(10, 7, 1, true)]
        [InlineData(10, 7, 5, false)]
        [InlineData(10, 7, 6, false)]
        [InlineData(10, 1, 4, true)]
        [InlineData(5, 2, 4, true)]
        public void CheckIsVictor_ReturnsExpected(int strength1, int strength2, int courage2, bool expectedResult)
        {
            // Arrange
            var t1 = new Transformer { Strength = strength1 };
            var t2 = new Transformer { Strength = strength2, Courage = courage2 };
            var sut = new StrengthBattleRule();

            // Act
            var result = sut.CheckIsVictor(t1, t2);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
