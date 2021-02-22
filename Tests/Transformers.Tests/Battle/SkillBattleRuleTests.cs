using System;
using Transformers.Model.Entities;
using Transformers.Model.Services.War.BattleRules;
using Xunit;

namespace Transformers.Tests.Battle
{
    public class SkillBattleRuleTests
    {
        // Rule: If Transformer A’s skill rating exceeds Transformer B's rating by 5 or more, Transformer A wins the fight.

        [Fact]
        public void CheckIsVictor_ThrowsArgumentNullException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(() => new SkillBattleRule().CheckIsVictor(null, new Transformer()));
            Assert.Throws<ArgumentNullException>(() => new SkillBattleRule().CheckIsVictor(new Transformer(), null));
            Assert.Throws<ArgumentNullException>(() => new SkillBattleRule().CheckIsVictor(null, null));
        }

        [Theory]
        [InlineData(10, 7, false)]
        [InlineData(5, 2, false)]
        [InlineData(6, 2, false)]
        [InlineData(7, 2, true)]
        [InlineData(10, 1, true)]
        public void CheckIsVictor_ReturnsExpected(int skill1, int skill2, bool expectedResult)
        {
            // Arrange
            var t1 = new Transformer { Skill = skill1 };
            var t2 = new Transformer { Skill = skill2 };
            var sut = new SkillBattleRule();

            // Act
            var result = sut.CheckIsVictor(t1, t2);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
