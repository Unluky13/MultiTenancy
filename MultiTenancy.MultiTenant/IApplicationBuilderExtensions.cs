using Microsoft.AspNetCore.Builder;
using MultiTenancy.MultiTenant.Middleware;
using System;

namespace MultiTenancy.MultiTenant
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMultiTenancyAuthentication(this IApplicationBuilder app, Action<MultiTenantAuthenticationMiddlewareOptions> optionsConfig)
        {
            var options = new MultiTenantAuthenticationMiddlewareOptions();
            optionsConfig(options);
            options.Validate();
            return app.UseMiddleware<MultiTenantAuthenticationMiddleware>(options);
        }

        public static IApplicationBuilder UseTenantInRouteValuesEndpoints(this IApplicationBuilder app)
        {
            return app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "empty",
                    pattern: "",
                    defaults: new { controller = "Home", action = "Index" });

                endpoints.MapControllerRoute(
                    name: "auth",
                    pattern: "Home/{action=Index}/{id?}",
                    defaults: new { controller = "Home" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: $"{{{MultiTenantAuthenticationMiddleware.RouteValueKey}}}/{{controller=Home}}/{{action=Index}}/{{id?}}");
            });
        }
    }
}
