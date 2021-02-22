using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Transformers.Model.Services;
using Transformers.Model.Services.War;
using Transformers.Model.Services.War.BattleRules;

namespace Transformers.Model
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWarService(this IServiceCollection services)
        {
            services
                .AddOptions<BattlePareIteratorSettings>()
                .Configure<IOptions<WarSettings>>((s, warOptions) => s.PageSize = warOptions.Value.BattlePareIterator.PageSize);

            return services
                .AddScoped<IWarService, WarService>()
                .AddSingleton<IBattleExecutor, BattleExecutor>()
                .AddScoped<IBattlePareIterator, BattlePareIterator>()
                .AddSingleton<IBattleRule, SpecialNamesBattleRule>()
                .AddSingleton<IBattleRule, StrengthBattleRule>()
                .AddSingleton<IBattleRule, SkillBattleRule>();
        }
    }
}
