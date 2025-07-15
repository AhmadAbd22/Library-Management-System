using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Library_Management.Models.Dtos
{
    public class UserLoginDto
    {
        [Required]
        [EmailAddress]
        public string email { get; set; } = string.Empty;

        [Required]
        [PasswordPropertyText]
        public string password { get; set; } = string.Empty;
    }
}
