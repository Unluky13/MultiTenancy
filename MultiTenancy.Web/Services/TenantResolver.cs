using Microsoft.AspNetCore.Http;
using MultiTenancy.Web.Data;
using System;
using MultiTenancy.Web.Services.Middleware;

namespace MultiTenancy.Web.Services
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
