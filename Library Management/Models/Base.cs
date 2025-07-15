namespace Library_Management.Models
{
    public class Base
    {
        public Guid Id { get; set; }
        public int isActive { get; set; }
        public Guid? actionBy { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public DateTime? deletedAt { get; set; }

    }
}
