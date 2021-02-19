using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Commands;
using Ductus.FluentDocker.Model.Containers;
using Ductus.FluentDocker.Services;
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
        private static readonly Lazy<string> _initLazy = new(InitDockerSql);
        private static DockerSqlSettings _dockerSqlSettings;


        public static IServiceCollection AddDbContext(this IServiceCollection services)
        {
            return services.AddDbContext<ITransformersDbContext, TransformersDbContext>((provider, builder) =>
            {
                var dataAccessSettings = provider.GetRequiredService<IOptions<DataAccessSettings>>().Value;

                if (dataAccessSettings.DataAccessType == DataAccessType.InMemory)
                {
                    builder.UseInMemoryDatabase(nameof(TransformersDbContext));
                }
                else if (dataAccessSettings.DataAccessType == DataAccessType.Classic)
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
                else if (dataAccessSettings.DataAccessType == DataAccessType.InDocker)
                {
                    _dockerSqlSettings = dataAccessSettings.DockerSql;
                    var connectionString = _initLazy.Value;
                    builder.UseSqlServer(new SqlConnection
                    {
                        ConnectionString = connectionString
                    });
                }
                else if (dataAccessSettings.DataAccessType == DataAccessType.Undefined)
                {
                    throw new ArgumentException($"Please configure DataAccess.DataAccessType.");
                }
                else if (!Enum.IsDefined(typeof(DataAccessType), dataAccessSettings.DataAccessType))
                {
                    throw new InvalidEnumArgumentException(nameof(dataAccessSettings.DataAccessType),
                        (int)dataAccessSettings.DataAccessType, typeof(DataAccessType));
                }
            });
        }

        [ExcludeFromCodeCoverage]
        private static string InitDockerSql()
        {
            var container =
                new Builder().UseContainer()
                    .UseImage(_dockerSqlSettings.Image)
                    .WithName(_dockerSqlSettings.ContainerName)
                    .ReuseIfExists()
                    .ExposePort(_dockerSqlSettings.HostPort, 1433)
                    .WithEnvironment("ACCEPT_EULA=Y", $"SA_PASSWORD={_dockerSqlSettings.SaPassword}")
                    .Build()
                    .Start();

            var isReady = false;

            while (!isReady)
            {
                var response = ExecuteSqlCmd(container,
                    "-h -1 -t 1 -Q \"SET NOCOUNT ON; Select SUM(state) from sys.databases\"");
                Debug.WriteLine(response.ToString());

                if (int.TryParse((response.Data.FirstOrDefault() ?? "").Trim(), out var status))
                {
                    isReady = status == 0;
                }

                if (!isReady)
                {
                    Thread.Sleep(500);
                }
            }

            return  $"Server=127.0.0.1,{_dockerSqlSettings.HostPort};Database=transformers;User ID=sa;Password={_dockerSqlSettings.SaPassword}";
        }

        [ExcludeFromCodeCoverage]
        private static CommandResponse<IList<string>> ExecuteSqlCmd(IContainerService container, string command)
        {
            return container.DockerHost.Execute(container.Id,
                $"{_dockerSqlSettings.SqlCmdExecutable} -S . -U sa -P {_dockerSqlSettings.SaPassword} -d master {command}");
        }
    }
}
