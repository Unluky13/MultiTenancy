using System;

namespace MultiTenancy.Web.Services.Middleware
{
    public class MultiTenantAuthenticationMiddlewareOptions
    {
        public string ClaimName { get; set; }

        internal void Validate()
        {
            if (string.IsNullOrWhiteSpace(ClaimName))
            {
                throw new ArgumentNullException(nameof(ClaimName));
            }
        }
    }
}