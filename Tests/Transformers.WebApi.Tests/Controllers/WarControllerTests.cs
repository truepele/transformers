using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Transformers.Model;
using Transformers.Model.Services;
using Transformers.Tests;
using Transformers.WebApi.Controllers;
using Xunit;

namespace Transformers.WebApi.Tests.Controllers
{
    public class WarControllerTests : TestsBase
    {
        private readonly WarController _sut;
        private readonly IWarService _warServiceStub;

        public WarControllerTests() : base(services =>
        {
            services.AddWarService()
                .AddSingleton<WarController>()
                .Replace(ServiceDescriptor.Singleton(Substitute.For<IWarService>()));
        })
        {
            _sut = GetService<WarController>();
            _warServiceStub = GetService<IWarService>();
        }

        [Fact]
        public async Task Get_ReturnsExpected()
        {
            // Arrange
            var id = 0;
            var expectedTransformers = TransformersFaker
                .RuleFor(t => t.Id, _ => id++)
                .Generate(10);
            _warServiceStub.PerformWar().Returns(expectedTransformers.ToAsyncEnumerable());

            // Act
            var result = await _sut.Get().ToListAsync();

            // Assert
            Assert.Equal(expectedTransformers.Select(t => t.Id),result);
        }
    }
}
