using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Library_Management.Models
{
    public class BorrowedBooks : Base
    {
        [Required]
        public Guid bookId { get; set; }
        [ForeignKey("bookId")]
        public virtual Book? book { get; set; }

        [Required]
        public Guid userId { get; set; }
        [ForeignKey("userId")]
        public DateTime rentDate { get; set; }
        public DateTime returnDate { get; set; }

        [Required]
        public int rentPrice { get; set; }
    }
}
