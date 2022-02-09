using Microsoft.Extensions.Caching.Memory;

namespace MultiTenancy.Web.Services
{
    public interface ITenantMemoryCache : IMemoryCache
    {
    }
}