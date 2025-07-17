using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Library_Management.Models
{
    public class ResponseDto
    {
        public dynamic? data { get; set; }
        public bool? status { get; set; }
        public string? statusCode { get; set; }
        public string? message { get; set; }
    }

    #region User

    public class UserDto
    {
        public string Id { get; set; }
        public string EncId { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
    }

    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
    }

    public class AddUserViewModel
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters.")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters.")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be 11 digits.")]
        public string Contact { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public int Role { get; set; }

        [Required]
        [PasswordPropertyText]
        public string Password { get; set; } = string.Empty;
    }

    public class EditUserViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters.")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters.")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be 11 digits.")]
        public string Contact { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public int Role { get; set; }
    }

    #endregion

    #region Book

    public class AddBookViewModel
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters.")]
        [StringLength(50)]
        public string Author { get; set; } = string.Empty;

        [Required]
        public int RentPrice { get; set; }

        [Required]
        public int BookQantity { get; set; }
    }

    public class BookViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters.")]
        [StringLength(50)]
        public string Author { get; set; } = string.Empty;

        [Required]
        public int RentPrice { get; set; }

        [Required]
        public int BookQantity { get; set; }
    }

    #endregion

    #region ContentFile

    public class ViewContentFile
    {
        public int? Id { get; set; }
        public string? EncId { get; set; }
        public string? FileName { get; set; }
    }

    #endregion
}
