using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Library_Management.Models.Dtos
{
    public class SignUpDto
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters.")]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters.")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be 11 digits.")]
        public string Phone { get; set; } = string.Empty;

        public string? Address { get; set; } = string.Empty;

        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City can only contain letters.")]
        public string ? City { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [PasswordPropertyText]
        public string Password { get; set; } = string.Empty;

        public int Role { get; set; }

    }
}
