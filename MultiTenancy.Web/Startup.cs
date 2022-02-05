using Autofac;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiTenancy.Auth;
using MultiTenancy.MultiTenant;
using MultiTenancy.MultiTenant.Strategies;
using MultiTenancy.Web.AutoFac;
using MultiTenancy.Web.Services;
using System.Linq;

namespace MultiTenancy.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(opts => opts.Filters.Add(new AuthorizeFilter()));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opts =>
                {
                    opts.Cookie.Name = "Auth";
                    opts.LoginPath = "/Home/Index";
                    opts.LogoutPath = "/Home/Logout";
                });

            services.AddAutofacMultitenantRequestServices();
            services.AddRouting();

            var strategyOptions = Configuration.GetSection("TenantStrategy").Get<RouteValuesTenantStrategyOptions>();
            services.AddRouteValueStrategy(x => x.PathBase = strategyOptions.PathBase);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac here. Don't
            // call builder.Populate(), that happens in AutofacServiceProviderFactory
            // for you.
            builder.RegisterAssemblyModules(GetType().Assembly);
        }

        public static MultitenantContainer ConfigureMultitenantContainer(IContainer container)
        {
            // This is the MULTITENANT PART. Set up your tenant-specific stuff here.
            var strategy = container.Resolve<ITenantIdentificationStrategy>();
            var mtc = new MultitenantContainer(strategy, container);

            using (var authContext = container.Resolve<AuthContext>())
            {
                SeedData(authContext);
                var services = new ITenantService[]
                {
                    new EnglishTenantService(),
                    new WelshTenantService(),
                    new FrenchTenantService()
                };

                var index = 0;
                foreach (var tenant in authContext.Tenants.ToList())
                {
                    mtc.ConfigureTenant(tenant.Name.ToUpper(), cb =>
                    {
                        cb.RegisterType(services[index].GetType()).As<ITenantService>();
                    });
                    index++;
                }
                mtc.ConfigureTenant("MultiTenant", cb =>
                {
                    cb.RegisterType(services[index].GetType()).As<ITenantService>();
                });
            }

            return mtc;
        }

        private static void SeedData(AuthContext context)
        {
            var tenant1 = new Tenant() { Id = 1, Name = "Tenant 1" };
            var tenant2 = new Tenant() { Id = 2, Name = "Tenant 2" };
            context.Tenants.AddRange(tenant1, tenant2);

            var user1 = new User() { Id = 1, Username = "Single" };
            user1.Tenants.Add(new TenantUser() { User = user1, Tenant = tenant1 });
            var user2 = new User() { Id = 2, Username = "Multi" };
            user2.Tenants.Add(new TenantUser() { User = user2, Tenant = tenant1 });
            user2.Tenants.Add(new TenantUser() { User = user2, Tenant = tenant2 });
            context.Users.AddRange(user1, user2);

            context.SaveChanges();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMultiTenancyAuthentication(x => x.ClaimName = Models.Home.ClaimTypes.Tenant);

            app.UseTenantInRouteValuesEndpoints();
        }
    }
}
