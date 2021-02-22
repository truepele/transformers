using System;
using Transformers.Model.Entities;
using Transformers.Model.Enums;
using Transformers.WebApi.Dto;
using Transformers.WebApi.Infrastructure.Validators;
using Xunit;

namespace Transformers.WebApi.Tests.Infrastructure.Validators
{
    public class UpdateTransformerValidatorTests
    {
        private readonly UpdateTransformerValidator _validator = new UpdateTransformerValidator();
        private readonly UpdateTransformerDto _dto = new()
        {
            Allegiance = Allegiance.Autobot,
            Courage = 1,
            Endurance = 2,
            Firepower = 3,
            Intelligence = 4,
            Name = Guid.NewGuid().ToString().Substring(0, Transformer.NameMaxLen),
            Rank = 5,
            Skill = 6,
            Speed = 7,
            Strength = 8,
            RowVersion = "aabb"
        };


        [Theory]
        [InlineData(Allegiance.Undefined)]
        [InlineData((Allegiance)(-100))]
        [InlineData((Allegiance)100)]
        [InlineData(Allegiance.Autobot, true)]
        [InlineData(Allegiance.Decepticon, true)]
        public void Allegiance_Validated(Allegiance allegiance, bool valid = false)
        {
            // Arrange
            _dto.Allegiance = allegiance;
            // Act
            var result = _validator.Validate(_dto);
            // Assert
            Assert.Equal(valid, result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("a", true)]
        public void RowVersion_Validated(string rowVersion, bool valid = false)
        {
            // Arrange
            _dto.RowVersion = rowVersion;
            // Act
            var result = _validator.Validate(_dto);
            // Assert
            Assert.Equal(valid, result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("12345678901234567890123456")]
        [InlineData("1234567890123456789012345", true)]
        [InlineData("a", true)]
        public void Name_Validated(string name, bool valid = false)
        {
            // Arrange
            _dto.Name = name;
            // Act
            var result = _validator.Validate(_dto);
            // Assert
            Assert.Equal(valid, result.IsValid);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        [InlineData(100)]
        [InlineData(11)]
        [InlineData(10, true)]
        [InlineData(9, true)]
        [InlineData(1, true)]
        public void Endurance_Validated(int val, bool valid = false)
        {
            // Arrange
            _dto.Endurance = val;
            // Act
            var result = _validator.Validate(_dto);
            // Assert
            Assert.Equal(valid, result.IsValid);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        [InlineData(100)]
        [InlineData(11)]
        [InlineData(10, true)]
        [InlineData(9, true)]
        [InlineData(1, true)]
        public void Firepower_Validated(int val, bool valid = false)
        {
            // Arrange
            _dto.Firepower = val;
            // Act
            var result = _validator.Validate(_dto);
            // Assert
            Assert.Equal(valid, result.IsValid);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        [InlineData(100)]
        [InlineData(11)]
        [InlineData(10, true)]
        [InlineData(9, true)]
        [InlineData(1, true)]
        public void Intelligence_Validated(int val, bool valid = false)
        {
            // Arrange
            _dto.Intelligence = val;
            // Act
            var result = _validator.Validate(_dto);
            // Assert
            Assert.Equal(valid, result.IsValid);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        [InlineData(100)]
        [InlineData(11)]
        [InlineData(10, true)]
        [InlineData(9, true)]
        [InlineData(1, true)]
        public void Rank_Validated(int val, bool valid = false)
        {
            // Arrange
            _dto.Rank = val;
            // Act
            var result = _validator.Validate(_dto);
            // Assert
            Assert.Equal(valid, result.IsValid);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        [InlineData(100)]
        [InlineData(11)]
        [InlineData(10, true)]
        [InlineData(9, true)]
        [InlineData(1, true)]
        public void Skill_Validated(int val, bool valid = false)
        {
            // Arrange
            _dto.Skill = val;
            // Act
            var result = _validator.Validate(_dto);
            // Assert
            Assert.Equal(valid, result.IsValid);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        [InlineData(100)]
        [InlineData(11)]
        [InlineData(10, true)]
        [InlineData(9, true)]
        [InlineData(1, true)]
        public void Speed_Validated(int val, bool valid = false)
        {
            // Arrange
            _dto.Speed = val;
            // Act
            var result = _validator.Validate(_dto);
            // Assert
            Assert.Equal(valid, result.IsValid);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        [InlineData(100)]
        [InlineData(11)]
        [InlineData(10, true)]
        [InlineData(9, true)]
        [InlineData(1, true)]
        public void Strength_Validated(int val, bool valid = false)
        {
            // Arrange
            _dto.Strength = val;
            // Act
            var result = _validator.Validate(_dto);
            // Assert
            Assert.Equal(valid, result.IsValid);
        }
    }
}
