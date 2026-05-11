using Azure;
using System.Collections.Generic;

namespace BlazorAppFood.Models
{
    public class Category
    {
        public int IdCategory { get; set; }
        public string Description { get; set; }
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
