using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Library_Management.Models.Dtos
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [PasswordPropertyText]
        public string Password { get; set; } = string.Empty;
    }
}
