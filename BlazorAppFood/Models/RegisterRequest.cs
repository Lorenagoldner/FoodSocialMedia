using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BlazorAppFood.Models
{
	public class RegisterRequest()
	{
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email is too long")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Por favor escolhe uma pergunta de segurança.")]
        public string SecurityQuestion { get; set; } = "";

        [Required(ErrorMessage = "A resposta de segurança é obrigatória.")]
        [MinLength(2, ErrorMessage = "A resposta deve ter pelo menos 2 caracteres.")]
        public string SecurityAnswer { get; set; } = "";
    }
}
