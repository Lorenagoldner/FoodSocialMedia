using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppFood.Models
{
    public class Posts
    {
        public int IdPost { get; set; }
        public int IdUser { get; set; }
        public int IdRecipe { get; set; }

        public Recipe Recipe { get; set; } = new Recipe();

    }
}
