using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MultiTenancy.Web.Data;
using MultiTenancy.Web.Services;

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

            services.AddRouting();

            services.AddMultiTenantDatabases<TenantResolver>(Configuration.GetConnectionString("Auth"));

            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddScoped<ITenantMemoryCache, TenantCache>();
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

            app.UseRouting();

            app.UseAuthentication();
            app.UseMultiTenancyAuthentication(x =>
            {
                x.ClaimName = Models.Home.ClaimTypes.Tenant;
            });

            app.UseAuthorization();

            app.UseTenantInRouteValuesEndpoints();
        }
    }
}
