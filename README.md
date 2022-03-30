# Multitenancy Example
This project demonstrates how an application can be made into a multi tenancy application.

# Databases
We are going to be using n+1 databases, where n is the total number of tenants.

Each tenant will have its own database to hold all its information separately to ensure there is no possible way of accessing a different tenant's data. This is modelled by the [TenantContext](https://github.com/Unluky13/MultiTenancy/blob/master/MultiTenancy.Web/Data/Tenant/TenantContext.cs).

The extra database is used to store all the users, tenants and links between the two. In this scenario, users can be linked to multiple tenants and has the ability to easily switch between tenants once logged in. Information about the tenant will store a friendly name and the connection string for the tenant's database. This is modelled by the [AuthContext](https://github.com/Unluky13/MultiTenancy/blob/master/MultiTenancy.Web/Data/Auth/AuthContext.cs).

## DbContext Injection
Each time you want to inject either of the DbContexts, it needs to have the correct connection details passed in. 

Rather than trying to set up the dependency injection to inject the correct DbOptions based on the current tenant or having the requesting service telling the context what the tenant is, it uses a [DbOptionsFactory](https://github.com/Unluky13/MultiTenancy/blob/master/MultiTenancy.Web/Data/DbOptionsFactory.cs) that will either use the connection string from appsettings for the AuthContext type, or get the connection string from the AuthContext for the current tenant for the TenantContext. This Factory is then injected into the constructors and each context asks for the correct options.

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

Having the tenant in the route makes it simple to create a service to get the current tenant:

```
  public class TenantResolver 
  {
      private readonly IHttpContextAccessor _httpContext;

      public TenantResolver(IHttpContextAccessor httpContext)
      {
          _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
      }

      public string Resolve()
      {
          return _httpContext.HttpContext.Request.RouteValues[MultiTenantAuthenticationMiddleware.RouteValueKey].ToString();
      }
  }
```

# Authorisation
To make sure the current user has permission to access the current tenant, we will be using a claim for each tenant the user has access to. 

```
  var claims = new List<Claim>();
  claims.Add(new Claim(System.Security.Claims.ClaimTypes.Name, user.Username));
  foreach (var tenant in user.Tenants)
  {
      claims.Add(new Claim(Models.Home.ClaimTypes.Tenant, tenant.Tenant.Name.ToUpper()));
      claims.Add(new Claim(Models.Home.ClaimTypes.TenantFriendly, tenant.Tenant.Name));
  }
```

We can then add some [middleware](https://github.com/Unluky13/MultiTenancy/blob/master/MultiTenancy.Web/Services/Middleware/MultiTenantAuthenticationMiddleware.cs) to check to see if the appropriate claim is held by the user and return a 401 if it is not.

# Caching

It is likely that data will want to be cached at either a global level or at a tenant level. To make this easy, an IMemoryCache can be injected for a global cache or an ITenantMemoryCache for tenant specific caching.

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
# Hyperlinks

Due to the tenant being in the route data, there is no need to specify it in any hyperlinks unless it is changing the current tenant:

```
  <a class="nav-link" asp-controller="Dashboard" asp-action="Other">Same Tenant</a>
  <a class="dropdown-item" asp-route-tenant="Tenant 2">Different Tenant</a>
```

# Considerations

## Configuration moved out of AppSettings and into the database
With there only being a single install, the configuration would need to be stored in a database to make it easy to add new tenants in the future without having to configure the whole application, e.g. create a new tenant databse with config in it and add a new tenant to the auth database.

## Performance
The application would need to handle multiple tenants using a single applciation or be able to scale vertically or preferably horizontally. Otherwise there is no benefit to multitenanc and it would only cause additional issues.

# Summary
This implementation of multenancy enables developers not to worry about the application being multitenant unless working on tenant specific functionality.
