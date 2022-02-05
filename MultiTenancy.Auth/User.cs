using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiTenancy.Auth
{
    [Table("USER")]
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public ICollection<TenantUser> Tenants { get; set; } = new List<TenantUser>();
    }
}
