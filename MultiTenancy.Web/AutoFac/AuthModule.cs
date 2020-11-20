using Autofac;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Auth;

namespace MultiTenancy.Web.AutoFac
{
    public class AuthModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<DbContextOptions>(_ =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<AuthContext>();
                optionsBuilder.UseInMemoryDatabase("Auth");
                return optionsBuilder.Options;
            });

            builder.RegisterType<AuthContext>()
                .AsSelf();

            builder.RegisterType<RouteValuesTenantStrategy>()
                .AsImplementedInterfaces();
        }
    }
}
