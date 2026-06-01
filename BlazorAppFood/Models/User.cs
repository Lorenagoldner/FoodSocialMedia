using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppFood.Models
{
    public class User
    {
        public int Id_User { get; set; }
        public string Username { get; set; }
        public string  Email { get; set; }
        public string Password { get; set; }
        public string UserPhoto { get; set; }
    }
}
