using Autofac.Multitenant;
using Microsoft.Extensions.DependencyInjection;
using MultiTenancy.MultiTenant.Strategies;
using System;

namespace MultiTenancy.MultiTenant
{
    public static class IServiceCollectionExtensions
    {
        public static void AddRouteValueStrategy(this IServiceCollection services, Action<RouteValuesTenantStrategyOptions> optionsConfig)
        {
            var options = new RouteValuesTenantStrategyOptions();
            optionsConfig(options);
            options.Validate();
            services.AddSingleton(options);

            services.AddTransient<ITenantIdentificationStrategy, RouteValuesTenantStrategy>();

        }
    }
}
