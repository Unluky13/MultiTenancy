using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiTenancy.Auth
{
    [Table("TENANT")]
    public class Tenant
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<TenantUser> Users { get; set; }
    }
}
