using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace BlazorAppFood.Models
{
    public class LoginRequest
    {
        [Required]
        [StringLength(50)]
        [EmailAddress]

        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }
    }
}
