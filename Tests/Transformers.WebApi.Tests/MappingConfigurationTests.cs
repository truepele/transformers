using AutoMapper;
using Transformers.WebApi.Infrastructure.Mappings;
using Xunit;

namespace Transformers.WebApi.Tests
{
    public class MappingConfigurationTests
    {
        [Fact]
        public void TransformerProfileValid()
        {
            // Arrange
            var configuration = new MapperConfiguration(cfg =>
                cfg.AddProfile(new TransformerProfile()));

            // Assert
            configuration.AssertConfigurationIsValid();
        }
    }
}
