using System.Linq;
using NSubstitute;
using Transformers.Model.Entities;
using Transformers.Model.Services.War;
using Xunit;

namespace Transformers.Tests.Battle
{
    public class BattleExecutorTests
    {
        [Fact]
        public void ExecuteBattle_ExecutesAllRulesThenReturnsNull()
        {
            // Arrange
            var t1 = new Transformer();
            var t2 = new Transformer();
            var rules = Enumerable.Range(1, 5).Select(_ => Substitute.For<IBattleRule>()).ToList();
            var sut = new BattleExecutor(rules);

            // Act
            var result = sut.ExecuteBattle(t1, t2);

            // Assert
            Assert.Null(result);
            Assert.All(rules, rule =>
            {
                rule.Received().CheckIsVictor(t1, t2);
                rule.Received().CheckIsVictor(t2, t1);
            });
        }

        [Fact]
        public void ExecuteBattle_ReturnsT2IfT1Null()
        {
            // Arrange
            Transformer t1 = null;
            var t2 = new Transformer();
            var rules = Enumerable.Empty<IBattleRule>();
            var sut = new BattleExecutor(rules);

            // Act
            var result = sut.ExecuteBattle(t1, t2);

            // Assert
            Assert.Equal(t2, result);
        }

        [Fact]
        public void ExecuteBattle_ReturnsT1IfT2Null()
        {
            // Arrange
            Transformer t2 = null;
            var t1 = new Transformer();
            var rules = Enumerable.Empty<IBattleRule>();
            var sut = new BattleExecutor(rules);

            // Act
            var result = sut.ExecuteBattle(t1, t2);

            // Assert
            Assert.Equal(t1, result);
        }

        [Theory]
        [InlineData(5, 2)]
        [InlineData(6, 2)]
        [InlineData(6, 0)]
        [InlineData(5, 4)]
        public void ExecuteBattle_ExecutesRulesUntilPositiveForT2(int rulesAmount, int positiveRuleIndex)
        {
            // Arrange
            var t1 = new Transformer();
            var t2 = new Transformer();
            var rules = Enumerable.Range(1, rulesAmount).Select(_ => Substitute.For<IBattleRule>()).ToList();
            rules[positiveRuleIndex].CheckIsVictor(t2, t1).Returns(true);
            var sut = new BattleExecutor(rules);

            // Act
            var result = sut.ExecuteBattle(t1, t2);

            // Assert
            Assert.Equal(t2, result);
            var cnt = 0;
            Assert.All(rules, rule =>
            {
                if (cnt++ <= positiveRuleIndex)
                {
                    rule.Received().CheckIsVictor(t1, t2);
                    rule.Received().CheckIsVictor(t2, t1);
                }
                else
                {
                    rule.DidNotReceiveWithAnyArgs().CheckIsVictor(default, default);
                }
            });
        }

        [Theory]
        [InlineData(5, 2)]
        [InlineData(6, 2)]
        [InlineData(6, 0)]
        [InlineData(5, 4)]
        public void ExecuteBattle_ExecutesRulesUntilPositiveForT1(int rulesAmount, int positiveRuleIndex)
        {
            // Arrange
            var t1 = new Transformer();
            var t2 = new Transformer();
            var rules = Enumerable.Range(1, rulesAmount).Select(_ => Substitute.For<IBattleRule>()).ToList();
            rules[positiveRuleIndex].CheckIsVictor(t1, t2).Returns(true);
            var sut = new BattleExecutor(rules);

            // Act
            var result = sut.ExecuteBattle(t1, t2);

            // Assert
            Assert.Equal(t1, result);
            var cnt = 0;
            Assert.All(rules, rule =>
            {
                if (cnt < positiveRuleIndex)
                {
                    rule.Received().CheckIsVictor(t1, t2);
                    rule.Received().CheckIsVictor(t2, t1);
                }
                else if (cnt == positiveRuleIndex)
                {
                    rule.Received().CheckIsVictor(t1, t2);
                    rule.Received().DidNotReceive().CheckIsVictor(t2, t1);
                }
                else
                {
                    rule.DidNotReceiveWithAnyArgs().CheckIsVictor(default, default);
                }

                cnt++;
            });
        }
    }
}
