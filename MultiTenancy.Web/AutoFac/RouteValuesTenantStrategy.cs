using Autofac.Multitenant;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;

namespace MultiTenancy.Web.AutoFac
{

    public class RouteValuesTenantStrategy : ITenantIdentificationStrategy
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly RouteValuesTenantStrategyOptions _options;

        public RouteValuesTenantStrategy(IHttpContextAccessor httpContextAccessor, IOptions<RouteValuesTenantStrategyOptions> options)
        {
            _httpContextAccessor = httpContextAccessor;
            _options = options.Value;
        }

        public bool TryIdentifyTenant(out object tenantId)
        {
            tenantId = null;
            try
            {
                if (_httpContextAccessor?.HttpContext?.Request != null)
                {
                    foreach(var part in _httpContextAccessor?.HttpContext.Request.Path.Value.Split("/", StringSplitOptions.RemoveEmptyEntries))
                    {
                        if(!part.Equals(_options.PathBase, StringComparison.OrdinalIgnoreCase))
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
