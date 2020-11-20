using Autofac.Multitenant;
using Microsoft.AspNetCore.Http;
using System;

namespace MultiTenancy.Web.AutoFac
{
    public class RouteValuesTenantStrategy : ITenantIdentificationStrategy
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RouteValuesTenantStrategy(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool TryIdentifyTenant(out object tenantId)
        {
            tenantId = null;
            try
            {
                if (_httpContextAccessor?.HttpContext?.Request != null)
                {
                    tenantId = _httpContextAccessor?.HttpContext.Request.Path.Value.Split("/", StringSplitOptions.RemoveEmptyEntries)[0];
                    tenantId = System.Web.HttpUtility.UrlDecode((string)tenantId);
                }
            }
            catch
            {
                // Happens at app startup in IIS 7.0
            }
            return tenantId != null;
        }
    }
}
