using System;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Transformers.Model;
using Transformers.WebApi.Infrastructure;
using Transformers.WebApi.Settings;
using Xunit;

namespace Transformers.WebApi.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddDbContext_RegistersContext_ForInMemory()
        {
            // Arrange
            var services = new ServiceCollection();
            services.Configure<DataAccessSettings>(settings =>
            {
                settings.DataAccessType = DataAccessType.InMemory;
                settings.SqlConnectionStringBuilder = null;
            });

            // Act
            var result = services.AddDbContext().BuildServiceProvider().GetRequiredService<ITransformersDbContext>();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void AddDbContext_RegistersContext_ForClassic()
        {
            // Arrange
            var services = new ServiceCollection();
            services.Configure<DataAccessSettings>(settings =>
            {
                settings.DataAccessType = DataAccessType.Classic;
                settings.SqlConnectionStringBuilder = new SqlConnectionStringBuilder("User Id=myUsername;Password=myPassword");
            });

            // Act
            var result = services.AddDbContext().BuildServiceProvider().GetRequiredService<ITransformersDbContext>();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void AddDbContext_Throws_ForUndefinedDataAccessType()
        {
            // Arrange
            var services = new ServiceCollection();
            services.Configure<DataAccessSettings>(_ => { });

            // Act / Assert
            Assert.Throws<ArgumentException>(() => services.AddDbContext().BuildServiceProvider().GetRequiredService<ITransformersDbContext>());
        }

        [Fact]
        public void AddDbContext_Throws_ForOutOfRangeDataAccessType()
        {
            // Arrange
            var services = new ServiceCollection();
            services.Configure<DataAccessSettings>(s => s.DataAccessType = (DataAccessType)100);

            // Act / Assert
            Assert.Throws<InvalidEnumArgumentException>(() => services.AddDbContext().BuildServiceProvider().GetRequiredService<ITransformersDbContext>());
        }
    }
}
