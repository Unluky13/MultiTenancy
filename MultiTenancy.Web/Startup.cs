using Autofac;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MultiTenancy.Auth;
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
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            services.AddControllersWithViews(opts => opts.Filters.Add(new AuthorizeFilter()));

            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opts =>
                {
                    opts.Cookie.Name = "Auth";
                    opts.LoginPath = "/Home/Index";
                    opts.LogoutPath = "/Home/Logout";
                });

            services.AddAutofacMultitenantRequestServices();
            services.AddRouting();
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
                    new WelshTenantService()
                };

                var index = 0;
                foreach (var tenant in authContext.Tenants.ToList())
                {
                    mtc.ConfigureTenant(tenant.Name, cb =>
                    {
                        cb.RegisterType(services[index].GetType()).As<ITenantService>();
                    });
                    index++;
                }
            }

            return mtc;
        }


        private static void SeedData(AuthContext context)
        {
            var tenant1 = new Tenant() { Id = 1, Name = "Tenant 1" };
            var tenant2 = new Tenant() { Id = 2, Name = "Tenant 2" };
            context.Tenants.AddRange(tenant1, tenant2);

            var user1 = new User() { Id = 1, Username = "Single" };
            user1.Tenants.Add(new UserTenant() { User = user1, Tenant = tenant1 });
            var user2 = new User() { Id = 2, Username = "Multi" };
            user2.Tenants.Add(new UserTenant() { User = user2, Tenant = tenant1 });
            user2.Tenants.Add(new UserTenant() { User = user2, Tenant = tenant2 });
            context.Users.AddRange(user1, user2);

            context.SaveChanges();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UsePathBase("/test");
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "empty",
                    pattern: "",
                    defaults: new { controller = "Home", action = "Index" });

                endpoints.MapControllerRoute(
                    name: "auth",
                    pattern: "Home/{action=Index}/{id?}",
                    defaults: new { controller = "Home" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{tenant}/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
