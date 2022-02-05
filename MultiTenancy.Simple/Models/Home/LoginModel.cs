using System.ComponentModel.DataAnnotations;

namespace MultiTenancy.Simple.Models.Home
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
    }
}
