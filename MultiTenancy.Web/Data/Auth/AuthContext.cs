using Microsoft.EntityFrameworkCore;

namespace MultiTenancy.Web.Data.Auth
{
    public class AuthContext : DbContext
    {
        public AuthContext(DbOptionsFactory optionsFactory) : base(optionsFactory.Create(DbContextType.Auth))
        {

        }

        public DbSet<User> Users { get; set; }

        public DbSet<Tenant> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TenantUser>().HasKey(nameof(TenantUser.TenantId), nameof(TenantUser.UserId));
        }
    }
}
