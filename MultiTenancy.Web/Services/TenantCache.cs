using Microsoft.Extensions.Caching.Memory;
using MultiTenancy.Web.Data;
using System;

namespace MultiTenancy.Web.Services
{
    public class TenantCache : ITenantMemoryCache
    {
        private readonly IMemoryCache _cache;

        private readonly string _tenant;

        public TenantCache(IMemoryCache cache, ITenantResolver resolver)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _tenant = (resolver ?? throw new ArgumentNullException(nameof(resolver))).Resolve();
        }

        private class TenantKey
        {
            public string Tenant { get; set; }

            public object Key { get; set; }

            public override bool Equals(object obj)
            {
                if (!(obj is TenantKey key))
                {
                    return false;
                }

                return Key.Equals(key.Key) && Tenant.Equals(key.Tenant);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Tenant, Key);
            }
        }

        private TenantKey CreateTenantkey(object key)
        {
            return new TenantKey()
            {
                Key = key,
                Tenant = _tenant
            };
        }

        public ICacheEntry CreateEntry(object key)
        {
            return _cache.CreateEntry(CreateTenantkey(key));
        }

        public void Dispose()
        {
        }

        public void Remove(object key)
        {
            _cache.Remove(CreateTenantkey(key));
        }

        public bool TryGetValue(object key, out object value)
        {
            return _cache.TryGetValue(CreateTenantkey(key), out value);
        }
    }
}
