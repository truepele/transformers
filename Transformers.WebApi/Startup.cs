using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Transformers.DataAccess.Services;
using Transformers.Model;
using Transformers.Model.Services;
using Transformers.Model.Services.War;
using Transformers.WebApi.Infrastructure;
using Transformers.WebApi.Infrastructure.Mappings;
using Transformers.WebApi.Infrastructure.Validators;
using Transformers.WebApi.Settings;

namespace Transformers.WebApi
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
                .AddFluentValidation(fv =>
                    fv.RegisterValidatorsFromAssemblyContaining<NewTransformerValidator>());
            services.AddHealthChecks();
            services.AddSwaggerGen();

            services
                .AddAutoMapper(typeof(TransformerProfile))
                .AddSingleton<AzureServiceTokenProvider>()
                .AddHttpContextAccessor()
                .AddSingleton<Func<IDbConnection>>(p => () =>
                    (p.GetRequiredService<IHttpContextAccessor>().HttpContext.RequestServices.GetRequiredService<ITransformersDbContext>() as DbContext).Database.GetDbConnection())
                .AddSingleton<IOverallRatingCalcService, OverallRatingCalcServiceStoredProc>()
                .AddWarService()
                .AddDbContext();

            services.Configure<DataAccessSettings>(Configuration.GetSection("DataAccess"));
            services.Configure<WarSettings>(Configuration.GetSection("War"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            if (env.IsDevelopment())
            {
                app
                    .UseSwagger()
                    .UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Transformers API V1");
                        c.RoutePrefix = string.Empty;
                    });

                using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
                if (scope.ServiceProvider.GetRequiredService<IOptions<DataAccessSettings>>().Value.DataAccessType !=
                    DataAccessType.InMemory)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    (scope.ServiceProvider.GetRequiredService<ITransformersDbContext>() as DbContext).Database.Migrate();
                }
            }
        }
    }
}
