using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using BlazorAppFood.Models;
using BlazorAppFood.Configuration;
using BlazorAppFood.Data;


namespace BlazorAppFood.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly SqlConnectionConfiguration _configuration;
        private readonly INotificationRepository _notificationRepository;

        public ChatRepository(
            SqlConnectionConfiguration configuration,
            INotificationRepository notificationRepository)
        {
            _configuration = configuration;
            _notificationRepository = notificationRepository;
        }

        public async Task<int> CreateComment(int idRecipe, int idUser, string message)
        {
            using (var conn = new SqlConnection(_configuration._value))

            {
                string sQuery = @"INSERT INTO Comments (IdRecipe, IdUser, Message) 
                                  VALUES (@idRecipe, @idUser, @message)";
              
                var result = await conn.ExecuteAsync(sQuery,
                    new
                    {
                        IdRecipe = idRecipe,
                        IdUser = idUser,
                        Message = message
                    });

                //buscar dono da receita
                string recipeQuery = @"SELECT Id_User FROM Recipe WHERE Id_Recipe = @idRecipe";
                int recipeOwnerId = await conn.ExecuteScalarAsync<int>
                    (recipeQuery, 
                    new { idRecipe = idRecipe });

                //Evita notificar o usuário se ele mesmo for o dono da receita
                if (recipeOwnerId != idUser)
                {
                    await _notificationRepository.CreateNotification(new Notification
                    {
                        RecipientUserId = recipeOwnerId,
                        ActorUserId = idUser,
                        Type = NotificationType.Comment,
                        Message = "Comentou na tua receita",
                        RelatedEntityId = idRecipe,
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    });
                }

                return result;
            }
        }

        public async Task<List<Comment>> LoadRecipeComments(int idRecipe)
        {
            using (var conn = new SqlConnection(_configuration._value))


            {
                string sQuery = @"Select Comments.*, Users.UserPhoto 
                                FROM Comments Inner JOIN Users ON Comments.IdUser = Users.Id_User 
                                WHERE Comments.IdRecipe = @idRecipe ";
                return (await conn.QueryAsync<Comment>(sQuery,
                    new
                    { IdRecipe = idRecipe }
                    )).ToList();
            }

        }
    }
}
