# Multitenancy Example
This project demonstrates how an application can be made into a multi tenancy application.

# Databases
We are going to be using n+1 databases, where n is the total number of tenants.

Each tenant will have its own database to hold all its information separately to ensure there is no possible way of accessing a different tenants data. This is modelled by the [TenantContext](https://github.com/Unluky13/MultiTenancy/blob/master/MultiTenancy.Web/Data/Tenant/TenantContext.cs).

The extra database is used to store al the users, tenants and links between the two. In this scenario, users can be linked to multiple tenants andd has the ability to easily switch between tenants once logged in. As information about the tenant will store a friendly name and the connection string for the tenants database. This is modelled by the [AuthContext](https://github.com/Unluky13/MultiTenancy/blob/master/MultiTenancy.Web/Data/Auth/AuthContext.cs).

## DbContext Injection
Each time you want to inject a Tenants DbContext, it needs to have the correct connection details passed in. 

Rather than trying to set up the dependency injection to inject the correct DbOptions, it uses a [DbOptionsFactory](https://github.com/Unluky13/MultiTenancy/blob/master/MultiTenancy.Web/Data/DbOptionsFactory.cs) that will either use the connection string from appsettings for the AuthContext type, or get the conection string from the AuthContext for the current tenant for the TenantContext. This Factory is then injected into the 2 DbContexts constructor and each context asks for the correct options.

```
  public class AuthContext : DbContext
  {
      public AuthContext(DbOptionsFactory optionsFactory) : base(optionsFactory.Create(DbContextType.Auth))
      {

      }
      ...
      
  public class TenantContext : DbContext
  {
      public TenantContext(DbOptionsFactory optionsFactory) : base(optionsFactory.Create(DbContextType.Tenant))
      {

      }
      ...
```

This means that injecting each of the DbContexts is trivial with no thought required of what the current tenant is.

```
  public class Service
  {
      public Service(AuthContext authContext, TenantContext tenantContext)
      {
      ...
```

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
