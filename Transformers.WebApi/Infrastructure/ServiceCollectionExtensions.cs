using System;
using System.ComponentModel;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Transformers.DataAccess;
using Transformers.Model;
using Transformers.WebApi.Settings;

namespace Transformers.WebApi.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services)
        {
            return services.AddDbContext<ITransformersDbContext, TransformersDbContext>((provider, builder) =>
            {
                var dataAccessSettings = provider.GetRequiredService<IOptions<DataAccessSettings>>().Value;

                if (dataAccessSettings.DataAccessType == DataAccessType.InMemory)
                {
                    builder.UseInMemoryDatabase(nameof(TransformersDbContext));
                }
                else if(dataAccessSettings.DataAccessType == DataAccessType.Classic)
                {
                    var connStringBuilder = dataAccessSettings.SqlConnectionStringBuilder;
                    var connection = new SqlConnection { ConnectionString = connStringBuilder.ConnectionString };
                    if (string.IsNullOrEmpty(connStringBuilder.Password) && !connStringBuilder.IntegratedSecurity)
                    {
                        connection.AccessToken = provider.GetRequiredService<AzureServiceTokenProvider>()
                            .GetAccessTokenAsync("https://database.windows.net/")
                            .GetAwaiter().GetResult();
                    }

                    builder.UseSqlServer(connection);
                }
                else if(dataAccessSettings.DataAccessType == DataAccessType.Undefined)
                {
                    throw new ArgumentException($"Please configure DataAccess.DataAccessType.");
                }
                else if(!Enum.IsDefined(typeof(DataAccessType), dataAccessSettings.DataAccessType))
                {
                    throw new InvalidEnumArgumentException(nameof(dataAccessSettings.DataAccessType), (int)dataAccessSettings.DataAccessType, typeof(DataAccessType));
                }
            });
        }
    }
}
