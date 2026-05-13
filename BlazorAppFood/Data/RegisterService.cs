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
    public class RegisterService : IRegisterService
    {
        // Database Connection
        private readonly SqlConnectionConfiguration _configuration;
        private readonly ISessionStorageService _sessionStorage;

        public RegisterService(SqlConnectionConfiguration configuration, ISessionStorageService sessionStorage)
        {
            _configuration = configuration;
            _sessionStorage = sessionStorage;
        }

        // Criação de novo utilizador e retorno do ID
        public async Task<bool> CreateRegist(string Username, string Email, string Password)
        {
            using (var conn = new SqlConnection(_configuration._value))

            // Inserir Verificações de DB aqui.

            {
                var parameters = new DynamicParameters();
                parameters.Add("Username", Username, DbType.String);
                parameters.Add("Email", Email, DbType.String);
                parameters.Add("Password", Password, DbType.String);

                const string query = @"INSERT INTO Users (Username, Email, Password) VALUES (@Username, @Email, @Password)";
                await conn.ExecuteAsync(query, new { Username, Email, Password }, commandType: CommandType.Text);
            }

            return true;
        }
        // Suposto log do user:
        public async Task<bool> LoginRegist(string Email, string Password)
        {

            using (var conn = new SqlConnection(_configuration._value))
            {
                string sQuery = @"Select COUNT(*) from Users Where Email = @Email and Password = @Password";
                int validation = conn.ExecuteScalar<int>(sQuery,
                                                         new
                                                         {
                                                             Email = Email,
                                                             Password = Password
                                                         });

                if (validation == 1)
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
        }

    }
}


