using System;

namespace MultiTenancy.MultiTenant.Strategies
{
    public class RouteValuesTenantStrategyOptions
    {
        /// <summary>
        /// Required to be set because the tenant identification happens 
        /// before PathBase is set on the HttpContext.
        /// </summary>
        public string PathBase { get; set; }

        internal void Validate()
        {
            if (string.IsNullOrWhiteSpace(PathBase))
            {
                throw new ArgumentNullException(nameof(PathBase));
            }
        }
    }
}
