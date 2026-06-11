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
                string sQuery = @"INSERT INTO Comments (IdRecipe, IdUser, Message, PublishedDate) 
                                  VALUES (@idRecipe, @idUser, @message, @publishedDate)";
              
                var result = await conn.ExecuteAsync(sQuery,
                    new
                    {
                        IdRecipe = idRecipe,
                        IdUser = idUser,
                        Message = message,
                        PublishedDate = DateTime.Now
                    });

                //buscar dono da receita
                string recipeQuery = @"SELECT Id_User FROM Recipe WHERE Id_Recipe = @idRecipe";
                int recipeOwnerId = await conn.ExecuteScalarAsync<int>
                    (recipeQuery, 
                    new { idRecipe = idRecipe });

                //Evita notificar o usuário se ele mesmo for o dono da receita
                if (recipeOwnerId != idUser)
                {
                    string actorQuery = @"SELECT Username 
                      FROM Users 
                      WHERE Id_User = @IdUser";

                    string actorName = await conn.ExecuteScalarAsync<string>(
                        actorQuery,
                        new { IdUser = idUser });

                    string recipeNameQuery = @"SELECT NameRecipe
                           FROM Recipe
                           WHERE Id_Recipe = @IdRecipe";

                    string recipeName = await conn.ExecuteScalarAsync<string>(
                        recipeNameQuery,
                        new { IdRecipe = idRecipe });

                    await _notificationRepository.CreateNotification(new Notification
                    {
                        RecipientUserId = recipeOwnerId,
                        ActorUserId = idUser,
                        Type = NotificationType.Comment,
                        Message = $"{actorName} comentou na tua receita {recipeName}",
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
                // OTIMIZAÇÃO (P3 - junho 2026):
                // Adicionado Users.Username AS UserName ao SELECT.
                // Antes a RecipePage fazia foreach + GetUserById por cada comment (N+1).
                // Agora vem tudo numa só query JOIN.
                string sQuery = @"SELECT Comments.*,
                                         Users.UserPhoto,
                                         Users.Username AS UserName
                                  FROM Comments
                                  INNER JOIN Users ON Comments.IdUser = Users.Id_User
                                  WHERE Comments.IdRecipe = @idRecipe
                                  ORDER BY Comments.PublishedDate ASC";

                return (await conn.QueryAsync<Comment>(sQuery, new { IdRecipe = idRecipe })).ToList();
            }
        }
    }
}
