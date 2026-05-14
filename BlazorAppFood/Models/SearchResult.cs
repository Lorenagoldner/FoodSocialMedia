using System.Collections.Generic;

namespace BlazorAppFood.Models
{
    public class SearchResult
    {
        public string Title { get; set; }

        public string Type { get; set; }

        public string Route { get; set; }

        private List<SearchResult> filteredResults = new();
    }
}
