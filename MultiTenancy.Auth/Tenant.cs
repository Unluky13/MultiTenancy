using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTenancy.Auth
{
    public class Tenant
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<UserTenant> Users { get; set; }
    }
}
