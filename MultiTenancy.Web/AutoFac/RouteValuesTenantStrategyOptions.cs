namespace MultiTenancy.Web.AutoFac
{
    public class RouteValuesTenantStrategyOptions
    {
        /// <summary>
        /// Required to be set because the tenant identification happens 
        /// before PathBase is set on the HttpContext.
        /// </summary>
        public string PathBase { get; set; }
    }
}
