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
        // Database Connection
        private readonly SqlConnectionConfiguration _configuration;

        public RegisterService(SqlConnectionConfiguration configuration, ISessionStorageService sessionStorage)
        {
            _configuration = configuration;
        }

        // Criação de novo utilizador e retorno do ID
        public async Task<bool> CreateRegist(string Username, string Email, string Password)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);
                const string query = @"INSERT INTO Users (Username, Email, Password) VALUES(@Username, @Email, @Password)";
                await conn.ExecuteAsync(query,
                    new
                    {
                        Username,
                        Email,
                        Password = hashedPassword
                    },
                    commandType: CommandType.Text);
               
                return true;
            }
        }
        // Suposto log do user:
        //public async Task<bool> LoginRegist(string Email, string Password)
        //{

        //    using (var conn = new SqlConnection(_configuration._value))
        //    {
        //        string sQuery = @"Select COUNT(*) from Users Where Email = @Email and Password = @Password";
        //        int validation = conn.ExecuteScalar<int>(sQuery,
        //                                                 new
        //                                                 {
        //                                                     Email = Email,
        //                                                     Password = Password
        //                                                 });

        //        if (validation == 1)
        //        {
        //            // Store email in sessionStorage after successful login
        //            await _sessionStorage.SetItemAsync("userEmail", Email);
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        }

    }



