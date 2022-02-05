using Microsoft.AspNetCore.Http;
using MultiTenancy.Data;
using MultiTenancy.MultiTenant.Middleware;
using System;

namespace MultiTenancy.Simple
{
    public class TenantResolver : ITenantResolver
    {
        private readonly IHttpContextAccessor _httpContext;

        public TenantResolver(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }

        public string Resolve()
        {
            return _httpContext.HttpContext.Request.RouteValues[MultiTenantAuthenticationMiddleware.RouteValueKey].ToString();
        }
    }
}
