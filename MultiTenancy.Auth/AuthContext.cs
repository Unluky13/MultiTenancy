using Microsoft.EntityFrameworkCore;
using System;

namespace MultiTenancy.Auth
{
    public class AuthContext : DbContext
    {
        public AuthContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

        public DbSet<Tenant> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserTenant>().HasKey(nameof(UserTenant.TenantId), nameof(UserTenant.UserId));
        }
    }
}
