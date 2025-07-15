namespace Library_Management.Models
{
    public class Book : Base
    {
        public string title { get; set; } = string.Empty;
        public string author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int rentPrice { get; set; }
        public int quantity { get; set; }
    }
}
