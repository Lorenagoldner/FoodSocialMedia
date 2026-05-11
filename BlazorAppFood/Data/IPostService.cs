using BlazorAppFood.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorAppFood.Services
{
    public interface IPostService
    {
        Task<List<Posts>> GetPostsByUserId(int userId);
    }
}
