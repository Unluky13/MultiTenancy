using System.Collections.Generic;

namespace MultiTenancy.Auth
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public ICollection<UserTenant> Tenants { get; set; } = new List<UserTenant>();
    }
}
