using Azure;
using System.Collections.Generic;

namespace BlazorAppFood.Models
{
    public class CategoryWithTags
    {
        public int IdCategory { get; set; }
        public string Description { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
