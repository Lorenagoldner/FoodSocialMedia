using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppFood.Models
{
    public class Ingredient
    {
        public int Id_Ingredient { get; set; }
        public string Name { get; set; }
        public int Id_Recipe { get; set; }

        public Ingredient(string name)
        {
            this.Name = name;
        }

        public Ingredient() { }
    }
}
