using System.ComponentModel.DataAnnotations.Schema;

namespace Library_Management.Models
{
    public class User : Base
    {
        [Column(TypeName = "nvarchar(255)")]
        public string firstName { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(255)")]
        public string lastName { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(255)")]
        public string username { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(255)")]
        public string email { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string password { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(255)")]
        public string? phoneNumber { get; set; }


        [Column(TypeName = "nvarchar(150)")]
        public string? city { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? address { get; set; }
        public int? role { get; set; }
    }
}
