﻿using Microsoft.Extensions.DependencyInjection;
using MultiTenancy.Web.Data.Auth;
using MultiTenancy.Web.Data.Tenant;

namespace MultiTenancy.Web.Data
{
    public static class ServiceCollectionExtenstions
    {
        public static void AddMultiTenantDatabases<TTenantResolver>(this IServiceCollection services, string authConnectionString)
            where TTenantResolver : class, ITenantResolver
        {
            services.AddScoped<ITenantResolver, TTenantResolver>();

            services.AddScoped<AuthContext, AuthContext>();
            services.AddScoped<TenantContext, TenantContext>();

            services.AddScoped<DbOptionsFactory, DbOptionsFactory>();

            services.AddScoped(_ => new AuthOptions() { ConnectionString = authConnectionString });
        }
    }
}
