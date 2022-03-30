# Multitenancy Example
This project demonstrates how an application can be turned into a multi tenancy application with a few simple changes.

It uses a database dedicated to user authentication and tenant registration and then a separate database for each tenant that contains the application informations.

Users can be linked to any number of tenants in this scenario.

# Tenant Resolution Strategy
The first decision is how to determine the current tenant for each request. In this project we will be using the request path to resolve the tenant.

For this we use a combination of different routing endpoints:

1. An empty route to handle the base application address
```
  endpoints.MapControllerRoute(
      name: "empty",
      pattern: "",
      defaults: new { controller = "Home", action = "Index" });
```
2. A route to handle the login page where there is no tenant in the route. This also prevents access to all other controllers without the tenant being present.
```
  endpoints.MapControllerRoute(
      name: "auth",
      pattern: "Home/{action=Index}/{id?}",
      defaults: new { controller = "Home" });
```
3. A route that includes the tenant before the default MVC route
```
  endpoints.MapControllerRoute(
      name: "default",
      pattern: $"{{{MultiTenantAuthenticationMiddleware.RouteValueKey}}}/{{controller=Home}}/{{action=Index}}/{{id?}}");
```

# Authorisation
To make sure the current user has permission to access the current tenant, we will be using a claim for each tenant the user has access to. We can then add some [middleware](https://github.com/Unluky13/MultiTenancy/blob/master/MultiTenancy.Web/Services/Middleware/MultiTenantAuthenticationMiddleware.cs) to check to see if the appropriate claim is held by the user and return a 401 if it is not.

# Databases

***Todo***

# Caching

It is likely that data will want to be caced at either a global level or at a tenant level. To make this easy, an IMemoryCache can be injected for a global cache or an ITenantMemoryCache for tenant specific caching.

ITenantMemoryCache has an [implementation](https://github.com/Unluky13/MultiTenancy/blob/master/MultiTenancy.Web/Services/TenantCache.cs) that has the same effect as prefixing any cache keys with the current tenant (albeit more complicated to handle any type of key and not just strings).

Due to [ITenantMemoryCache](https://github.com/Unluky13/MultiTenancy/blob/master/MultiTenancy.Web/Services/ITenantMemoryCache.cs) inheriting IMemoryCache, all the extensions for interacting with IMemoryCache can be used in the same way for ITenantMemoryCache.

```
  @inject ITenantMemoryCache TenantCache;
  var tenantCachedValue = TenantCache.GetOrCreate("Test", x =>
  {
      x.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
      return DateTime.Now;
  });
  
  @inject IMemoryCache GlobalCache;
  var globalCachedValue = GlobalCache.GetOrCreate("Test", x =>
  {
      x.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15);
      return DateTime.Now;
  });
```
