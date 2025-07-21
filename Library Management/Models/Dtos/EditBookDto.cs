namespace Library_Management.Models.Dtos
{
    public class EditBookDto
    {
        public Guid Id { get; set; }
        public string? title { get; set; }
        public string? author { get; set; }
        public string? ISBN { get; set; }
        public int totalQuantity { get; set; }
        public int rentPrice { get; set; }
    }
}
