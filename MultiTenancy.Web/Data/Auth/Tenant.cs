using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiTenancy.Web.Data.Auth
{
    [Table("TENANT")]
    public class Tenant
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Column("CONNECTION_STRING")]
        public string ConnectionString { get; set; }

        public ICollection<TenantUser> Users { get; set; }
    }
}
