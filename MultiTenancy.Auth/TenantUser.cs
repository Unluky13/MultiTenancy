using System.ComponentModel.DataAnnotations.Schema;

namespace MultiTenancy.Auth
{
    [Table("TENANT_USER")]
    public class TenantUser
    {
        [Column("USER_ID")]
        public int UserId { get; set; }

        public User User { get; set; }

        [Column("TENANT_ID")]
        public int TenantId { get; set; }

        public Tenant Tenant { get; set; }
    }
}
