using System.ComponentModel.DataAnnotations.Schema;

namespace MultiTenancy.Web.Data.Tenant
{
    [Table("NAME")]
    public class Name
    {
        public int Id { get; set; }

        public string Forename { get; set; }
    }
}
