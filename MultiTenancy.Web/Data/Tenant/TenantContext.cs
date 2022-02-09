using Microsoft.EntityFrameworkCore;

namespace MultiTenancy.Web.Data.Tenant
{
    public class TenantContext : DbContext
    {
        public TenantContext(DbOptionsFactory optionsFactory) : base(optionsFactory.Create(DbContextType.Tenant))
        {

        }

        public DbSet<Name> Names { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
