using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppFood.Models
{
    public class Recipe
    {
        public int Id_Recipe { get; set; }
        public int Id_User { get; set; }
        public string NameRecipe { get; set; }
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public int Prep_Time { get; set; }
        public int Cook_Time { get; set; }
        public string Preparation { get; set; }
        public byte[] Image { get; set; }
        public string Video { get; set; }

        public string Username { get; set; }
        public string UserPhoto { get; set; }
        public decimal AverageRating { get; set; }
        public List<Comment> Comments { get; set; }

        public string rating { get; set; }
        public DateTime PublishedDate { get; set; }

        public List<int> TagIds { get; set; } = new List<int>();
        public List<Tag> Tags { get; set; } = new List<Tag>();

        public int Servings { get; set; }
        public DifficultyLevel Difficulty { get; set; }

        public int FavoriteCount { get; set; }

        public string SearchText =>
         $"{NameRecipe} {Username} {string.Join(" ", Tags?.Select(t => t.NameTag) ?? new List<string>())}";
    }
}
