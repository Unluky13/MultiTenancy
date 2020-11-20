using System.ComponentModel.DataAnnotations;

namespace MultiTenancy.Web.Models.Home
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
    }
}
