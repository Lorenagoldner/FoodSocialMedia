using BCrypt.Net;
using BlazorAppFood.Models;
using Blazored.SessionStorage;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace BlazorAppFood.Data
{
    public class RegisterService : IRegisterService
    {
        private readonly SqlConnectionConfiguration _configuration;
        private readonly ISessionStorageService _sessionStorage;
        public RegisterService(SqlConnectionConfiguration configuration, ISessionStorageService sessionStorage)
        {
            _configuration = configuration;
            _sessionStorage = sessionStorage;
        }
        public async Task<bool> EmailExists(string email)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                const string query = "SELECT COUNT(1) FROM Users WHERE LOWER(Email) = LOWER(@Email)";
                int count = await conn.ExecuteScalarAsync<int>(query, new { Email = email });
                return count > 0;
            }
        }
        public async Task<bool> CreateRegist(string Username, string Email, string Password)
        {
            if (await EmailExists(Email))
            {
                return false;
            }
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);
            using (var conn = new SqlConnection(_configuration._value))
            {
                const string query = @"INSERT INTO Users (Username, Email, Password)
                    VALUES (@Username, @Email, @Password)";
                await conn.ExecuteAsync(query, new
                {
                    Username,
                    Email,
                    Password = hashedPassword
                });
            }
            return true;
        }
    }
}


