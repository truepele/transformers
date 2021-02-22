using System;
using Microsoft.Extensions.Options;
using Transformers.Model.Entities;
using Transformers.Model.Services.War;
using Transformers.Model.Services.War.BattleRules;
using Xunit;

namespace Transformers.Tests.Battle
{
    public class SpecialNamesRuleTests
    {
        // Rule: If Transformer A’s skill rating exceeds Transformer B's rating by 5 or more, Transformer A wins the fight.

        private readonly string[] _specialNames = { "SpecialName1", "SpecialName2", "SpecialName3" };
        private readonly SpecialNamesBattleRule _sut;

        public SpecialNamesRuleTests()
        {
            _sut = new SpecialNamesBattleRule(Options.Create(new WarSettings
            {
                SpecialNames = _specialNames
            }));
        }

        [Fact]
        public void CheckIsVictor_ThrowsArgumentNullException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(() => _sut.CheckIsVictor(null, new Transformer()));
        }

        [Theory]
        [InlineData("some name", "some name 2", false)]
        [InlineData("some name", "SpecialName1", false)]
        [InlineData("some name", "SpecialName2", false)]
        [InlineData("SpecialName1", "some name", true)]
        [InlineData("SpecialName2", "some name", true)]
        [InlineData("SpecialName3", "some name", true)]
        public void CheckIsVictor_ReturnsExpected(string name1, string name2, bool expectedResult)
        {
            // Arrange
            var t1 = new Transformer { Name = name1 };
            var t2 = new Transformer { Name = name2 };


            // Act
            var result = _sut.CheckIsVictor(t1, t2);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
