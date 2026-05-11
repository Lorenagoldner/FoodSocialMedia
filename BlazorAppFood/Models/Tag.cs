namespace BlazorAppFood.Models
{
    public class Tag
    {
        public int IdTag { get; set; }
        public string NameTag { get; set; }
        public int IdCategory { get; set; }
        public Category Category { get; set; }
    }
}
