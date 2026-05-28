using BlazorAppFood.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorAppFood.Repositories
{
    public interface IRegisterRepository
    {
        Task<bool> CreateRegist(string Username, string Email, string Password, string SecurityQuestion, string SecurityAnswer);
        Task<bool> EmailExists(string email);
    }
}