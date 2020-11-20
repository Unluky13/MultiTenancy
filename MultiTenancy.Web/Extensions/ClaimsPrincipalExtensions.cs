using System.Linq;
using System.Security.Claims;

namespace MultiTenancy.Web.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string[] Tenants(this ClaimsPrincipal principal)
        {
            return principal.Claims
                .Where(x => x.Type == Models.Home.ClaimTypes.Tenant)
                .Select(x => x.Value)
                .ToArray();
        }
    }
}
