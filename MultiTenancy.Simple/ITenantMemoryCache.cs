using Microsoft.Extensions.Caching.Memory;

namespace MultiTenancy.Simple
{
    public interface ITenantMemoryCache : IMemoryCache
    {
    }
}