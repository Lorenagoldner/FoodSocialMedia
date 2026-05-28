using BCrypt.Net;
using BlazorAppFood.Models;
using Blazored.SessionStorage;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using BlazorAppFood.Configuration;

namespace BlazorAppFood.Repositories
{
    public class RegisterRepository : IRegisterRepository
    {
        private readonly SqlConnectionConfiguration _configuration;
        private readonly ISessionStorageService _sessionStorage;

        public RegisterRepository(SqlConnectionConfiguration configuration, ISessionStorageService sessionStorage)
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

        public async Task<bool> CreateRegist(
            string Username,
            string Email,
            string Password,
            string SecurityQuestion,
            string SecurityAnswer)
        {
            if (await EmailExists(Email))
                return false;

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);

            string hashedAnswer = BCrypt.Net.BCrypt.HashPassword(SecurityAnswer.Trim().ToLower());

            using (var conn = new SqlConnection(_configuration._value))
            {
                const string query = @"
                    INSERT INTO Users (Username, Email, Password, SecurityQuestion, SecurityAnswerHash)
                    VALUES (@Username, @Email, @Password, @SecurityQuestion, @SecurityAnswerHash)";

                await conn.ExecuteAsync(query, new
                {
                    Username,
                    Email,
                    Password = hashedPassword,
                    SecurityQuestion,
                    SecurityAnswerHash = hashedAnswer
                });
            }

            return true;
        }
    }
}