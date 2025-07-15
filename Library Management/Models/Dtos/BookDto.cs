using System.ComponentModel.DataAnnotations;

namespace Library_Management.Models.Dtos
{
    public class BookDto
    {
        public Guid Id { get; set; }

        [Required]
        public string title { get; set; }
        public string author { get; set; }
        public int totalQuantity { get; set; }
        public string ISBN { get; set; }
    }
}
