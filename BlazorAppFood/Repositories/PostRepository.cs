using BlazorAppFood.Models;
using BlazorAppFood.Repositories;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppFood.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly IConfiguration _configuration;

        public PostRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("FoodSocialMediaDB"));
        }

        public async Task<List<Posts>> GetPostsByUserId(int userId)
        {
            using var connection = CreateConnection();

            var query = @"SELECT * FROM Posts WHERE Id_User = @UserId ORDER BY DatePublication DESC";

            var posts = await connection.QueryAsync<Posts>(query, new { UserId = userId });

            return posts.ToList();
        }
    }
}

