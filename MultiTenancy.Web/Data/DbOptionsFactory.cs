using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using MultiTenancy.Web.Data.Auth;
using MultiTenancy.Web.Data.Tenant;
using System;
using System.Linq;

namespace MultiTenancy.Web.Data
{
    public class DbOptionsFactory
    {
        private readonly AuthOptions _authOptions;

        private readonly IServiceProvider _serviceProvider;

        private readonly IMemoryCache _cache;

        private readonly ITenantResolver _tenantResolver;

        public DbOptionsFactory(AuthOptions authOptions, IServiceProvider serviceProvider, IMemoryCache cache, ITenantResolver tenantResolver)
        {
            _authOptions = authOptions ?? throw new ArgumentNullException(nameof(authOptions));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _tenantResolver = tenantResolver ?? throw new ArgumentNullException(nameof(tenantResolver));
        }

        public DbContextOptions Create(DbContextType type)
        {
            switch (type)
            {
                case DbContextType.Tenant:
                    return BuildTenantOptions();
                case DbContextType.Auth:
                    return BuildAuthOptions();
            }
            throw new ArgumentOutOfRangeException(nameof(type), $"Unhandled type: {type}");
        }

        private DbContextOptions BuildTenantOptions()
        {
            var tenant = _tenantResolver.Resolve();
            var key = $"ConnString_{tenant}";
            var optionsBuilder = new DbContextOptionsBuilder<TenantContext>();

            string connectionString;
            if (!_cache.TryGetValue(key, out connectionString))
            {
                var authContext = (AuthContext)_serviceProvider.GetRequiredService(typeof(AuthContext));
                connectionString = authContext.Tenants
                    .Where(x => x.Name.ToUpper() == tenant.ToUpper())
                    .Select(x => x.ConnectionString)
                    .SingleOrDefault();
                _cache.Set(key, connectionString);
            }
            optionsBuilder.UseSqlServer(connectionString);
            return optionsBuilder.Options;
        }

        private DbContextOptions BuildAuthOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AuthContext>();
            optionsBuilder.UseSqlServer(_authOptions.ConnectionString);
            return optionsBuilder.Options;
        }
    }
}
