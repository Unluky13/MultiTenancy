using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenancy.Web.Services
{
    public class EnglishTenantService : ITenantService
    {
        public string Hello()
        {
            return "Hello";
        }
    }
}
