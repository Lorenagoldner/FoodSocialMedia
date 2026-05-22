using BCrypt.Net;
using BlazorAppFood.Models;
using Blazored.SessionStorage;
using BlazorAppFood.Configuration;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace BlazorAppFood.Repositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly SqlConnectionConfiguration _configuration;
        private readonly ISessionStorageService _sessionStorage;

        public LoginRepository(SqlConnectionConfiguration configuration, ISessionStorageService sessionStorage)
        {
            _configuration = configuration;
            _sessionStorage = sessionStorage;
        }

        public async Task<bool> LogUser(string Email, string Password)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                string sQuery = @"SELECT * FROM Users WHERE Email = @Email";
                var user = await conn.QueryFirstOrDefaultAsync<User>(sQuery, new { Email });

                if (user == null) return false;

                bool isValid = BCrypt.Net.BCrypt.Verify(Password, user.Password);

                if (isValid)
                {
                    await _sessionStorage.SetItemAsync("userEmail", Email);
                    return true;
                }

                return false;
            }
        }
    }
}