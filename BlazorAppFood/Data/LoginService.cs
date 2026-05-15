using BCrypt.Net;
using BlazorAppFood.Models;
using Blazored.SessionStorage;
using Dapper;
using Microsoft.Data.SqlClient;
using Syncfusion.Blazor.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppFood.Data
{
    public class LoginService : ILoginService
    {
        private readonly SqlConnectionConfiguration _configuration;
        private readonly ISessionStorageService _sessionStorage;

        public LoginService(SqlConnectionConfiguration configuration, ISessionStorageService sessionStorage)
        {
            _configuration = configuration;
            _sessionStorage = sessionStorage;
        }
        public async Task<bool> LogUser(string Email, string Password)
        {
    //        throw new Exception(
    //BCrypt.Net.BCrypt.HashPassword("011624"));
            using (var conn = new SqlConnection(_configuration._value))


            {
                string sQuery = @"SELECT * FROM Users WHERE Email = @Email";
                var user = await conn.QueryFirstOrDefaultAsync<User>(sQuery,
                new { Email });

                if (user == null)
                {
                    return false;
                }
                bool isValid = BCrypt.Net.BCrypt.Verify(
                                Password,
                                user.Password);
                if (isValid)
                {
                    // Store email in sessionStorage after successful login
                    await _sessionStorage.SetItemAsync("userEmail", Email);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            //return true;

            //throw new NotImplementedException();
        }
    }
}
