using Autofac.Multitenant;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MultiTenancy.MultiTenant.Middleware
{
    internal class MultiTenantAuthenticationMiddleware
    {
        public const string RouteValueKey = "tenant";

        private readonly RequestDelegate _next;
        private readonly MultiTenantAuthenticationMiddlewareOptions _options;

        public MultiTenantAuthenticationMiddleware(RequestDelegate next, MultiTenantAuthenticationMiddlewareOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(HttpContext context, ITenantIdentificationStrategy tenantStrategy)
        {
            if (context.Request.RouteValues.ContainsKey(RouteValueKey))
            {
                var tenant = tenantStrategy.IdentifyTenant<string>();

                var user = context.User;
                if (!UserHasPermissionForTenant(tenant, user))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync($"You do not have permission to access '{tenant}'.");
                    return;
                }
            }

            await _next.Invoke(context);
        }

        private bool UserHasPermissionForTenant(string tenant, System.Security.Claims.ClaimsPrincipal user)
        {
            if (tenant == null)
            {
                return true;
            }

            if (user == null)
            {
                return false;
            }

            return user.HasClaim(_options.ClaimName, tenant.ToUpper());
        }
    }
}
