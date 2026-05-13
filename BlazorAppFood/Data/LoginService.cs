using BlazorAppFood.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Blazored.SessionStorage;

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
            using (var conn = new SqlConnection(_configuration._value))


            {
                string sQuery = @"Select COUNT(*) from Users Where Email = @Email and Password = @Password";
                int validation = conn.ExecuteScalar<int>(sQuery,
                                                        new { Email, Password });

                if (validation == 1)
                {
                    // Store email in sessionStorage after successful login
                    await _sessionStorage.SetItemAsync("email", Email);
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
