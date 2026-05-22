using BlazorAppFood.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorAppFood.Repositories
{
    public interface IRegisterRepository
    {
        Task<bool> CreateRegist(string Username, string Email, string Password);
        Task<bool> LoginRegist(string Email, string Password);
    }
}