﻿using System.Linq;
using System.Security.Claims;

namespace MultiTenancy.Simple.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string[] Tenants(this ClaimsPrincipal principal)
        {
            return principal.Claims
                .Where(x => x.Type == Models.Home.ClaimTypes.TenantFriendly)
                .Select(x => x.Value)
                .ToArray();
        }
    }
}
