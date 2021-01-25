using Autofac.Multitenant;
using Microsoft.AspNetCore.Http;
using System;

namespace MultiTenancy.MultiTenant.Strategies
{
    internal class RouteValuesTenantStrategy : ITenantIdentificationStrategy
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly RouteValuesTenantStrategyOptions _options;

        public RouteValuesTenantStrategy(IHttpContextAccessor httpContextAccessor, RouteValuesTenantStrategyOptions options)
        {
            _httpContextAccessor = httpContextAccessor;
            _options = options;
        }

        public bool TryIdentifyTenant(out object tenantId)
        {
            tenantId = null;
            try
            {
                if (_httpContextAccessor?.HttpContext?.Request != null)
                {
                    foreach (var part in _httpContextAccessor?.HttpContext.Request.Path.Value.Split("/", StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!part.Equals(_options.PathBase, StringComparison.OrdinalIgnoreCase))
                        {
                            tenantId = System.Web.HttpUtility.UrlDecode(part.ToUpper());
                            break;
                        }
                    }
                }
            }
            catch
            {
            }
            return !string.IsNullOrWhiteSpace(tenantId?.ToString());
        }
    }
}
