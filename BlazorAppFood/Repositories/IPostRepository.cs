using BlazorAppFood.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorAppFood.Repositories
{
    public interface IPostRepository
    {
        Task<List<Posts>> GetPostsByUserId(int userId);
    }
}
