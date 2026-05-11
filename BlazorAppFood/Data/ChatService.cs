using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using BlazorAppFood.Models;


namespace BlazorAppFood.Data
{
    public class ChatService : IChatService
    {
        private readonly SqlConnectionConfiguration _configuration;

        public ChatService(SqlConnectionConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> CreateComment(int idRecipe, int idUser, string message)
        {
            using (var conn = new SqlConnection(_configuration._value))

            {
                string sQuery = $"INSERT INTO Comments (IdRecipe, IdUser, Message) VALUES ({idRecipe}, {idUser}, '{message}')";
                return await conn.ExecuteAsync(sQuery);
            }
        }

        public async Task<List<Comment>> LoadRecipeComments(int idRecipe)
        {
            using (var conn = new SqlConnection(_configuration._value))


            {
                string sQuery = $"Select Comments.*, Users.UserPhoto FROM Comments " +
                    $"Inner JOIN Users ON Comments.IdUser = Users.Id_User " +
                    $"WHERE Comments.IdRecipe = '{idRecipe}' ";

                return (await conn.QueryAsync<Comment>(sQuery)).ToList();
            }

        }
    }
}
