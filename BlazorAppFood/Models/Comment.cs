using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppFood.Models
{
    public class Comment
    {
        public int IdComment {get; set;}
        public int IdUser { get; set; }
        public int IdRecipe { get; set; }
        public string Message { get; set; }
        public DateTime PublishedDate { get; set; }
        public string UserPhoto { get; set; }
        public string UserName { get; set; }
    }
}
