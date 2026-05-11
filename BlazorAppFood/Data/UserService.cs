using BlazorAppFood.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppFood.Data
{
    public class UserService : IUserService
    {
        //Database Connection
        private readonly SqlConnectionConfiguration _configuration;
        public UserService(SqlConnectionConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<User> GetUserInfo(string emailAddress)
        {
            User user = new User();

            using (var conn = new SqlConnection(_configuration._value))
            {
                string sQuery = "SELECT Id_User, Username, UserPhoto FROM Users WHERE Email = @Email";
                user = await conn.QuerySingleAsync<User>(sQuery, new { Email = emailAddress });
            }

            return user;
        }

        public async Task<bool> FollowUser(int currentUserId, int userIdToFollow)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                string query = "INSERT INTO User_Follower (Id_User, Id_Follower) VALUES (@IdUser, @IdFollower)";

                var affectedRows = await conn.ExecuteAsync(query, new
                {
                    IdUser = userIdToFollow,
                    IdFollower = currentUserId
                });

                return affectedRows > 0;
            }
        }

        public async Task<bool> IsFollowing(int currentUserId, int userIdToCheck)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                string query = "SELECT COUNT(1) FROM User_Follower WHERE Id_User = @UserId AND Id_Follower = @FollowerId";
                int count = await conn.ExecuteScalarAsync<int>(query, new
                {
                    UserId = userIdToCheck,
                    FollowerId = currentUserId
                });

                return count > 0;
            }
        }

        public async Task<bool> UnfollowUser(int currentUserId, int userIdToUnfollow)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                string query = "DELETE FROM User_Follower WHERE Id_User = @UserId AND Id_Follower = @FollowerId";

                var affectedRows = await conn.ExecuteAsync(query, new
                {
                    UserId = userIdToUnfollow,
                    FollowerId = currentUserId
                });

                return affectedRows > 0;
            }
        }

        public async Task<int> GetFollowerCount(int userId)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                string query = @"
            SELECT COUNT(*) 
            FROM User_Follower 
            WHERE Id_User = @UserId";

                return await conn.ExecuteScalarAsync<int>(query, new { UserId = userId });
            }
        }

        public async Task<List<User>> GetFollowedUsers(int userId)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                string query = @"
            SELECT U.Id_User, U.Username, U.UserPhoto
            FROM Users U
            INNER JOIN User_Follower UF ON U.Id_User = UF.Id_User
            WHERE UF.Id_Follower = @UserId";

                var users = await conn.QueryAsync<User>(query, new { UserId = userId });
                return users.ToList();
            }
        }

        public async Task<List<Recipe>> GetFollowingRecipes(string emailAddress)
        {
            // User user = new User();

            using (var conn = new SqlConnection(_configuration._value))
            {

                string query = "SELECT  Recipe.Id_Recipe, Recipe.NameRecipe, Recipe.Image, Recipe.Prep_Time, Recipe.Cook_Time, Users.Username, Users.UserPhoto, Recipe.AverageRating FROM Recipe " +
                                "Inner JOIN Users ON Recipe.Id_User = Users.Id_User " +
                                "WHERE Recipe.Id_User IN" +
                                    "(SELECT Id_User FROM User_Follower WHERE Id_Follower IN " +
                                        "(SELECT Id_User FROM Users WHERE Email = '" + emailAddress + "'))";

                //List<object> listRecipes = (await conn.QueryAsync<object>(query, commandType: CommandType.Text)).ToList();
                return (await conn.QueryAsync<Recipe>(query, commandType: CommandType.Text)).ToList();
            }

            //return user;
        }

        public async Task<IEnumerable<User>> UsersList()
        {
            IEnumerable<User> users;
            using (var conn = new SqlConnection(_configuration._value))
            {
                users = await conn.QueryAsync<User>("spUsers_GetAll", commandType: CommandType.StoredProcedure);
            }

            return users;
        }
        public async Task<bool> UpdateUser(User user)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                // Se a senha estiver vazia ou nula, não a atualizamos
                var query = @"UPDATE Users 
                      SET Username = @Username, 
                          Email = @Email, 
                          UserPhoto = @UserPhoto,
                          Password = CASE WHEN @Password IS NOT NULL AND @Password != '' THEN @Password ELSE Password END
                      WHERE Id_User = @Id_User";

                var affectedRows = await conn.ExecuteAsync(query, new
                {
                    user.Username,
                    user.Email,
                    user.Password,  // A senha só será alterada se ela não for nula ou vazia
                    user.UserPhoto,
                    user.Id_User
                });

                return affectedRows > 0;
            }
        }
        public async Task<User> GetUserByEmail(string email)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                string query = "SELECT * FROM Users WHERE Email = @Email";
                var user = await conn.QuerySingleOrDefaultAsync<User>(query, new { Email = email });
                return user;
            }
        }
        public async Task<User> GetUserById(int idUser)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                string query = "SELECT Id_User, Username, UserPhoto FROM Users WHERE Id_User = @Id";
                var user = await conn.QuerySingleOrDefaultAsync<User>(query, new { Id = idUser });
                return user;
            }
        }
        public async Task<bool> UploadPhoto(int userId, byte[] photoBytes)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                var query = "UPDATE Users SET UserPhoto = @UserPhoto WHERE Id_User = @UserId";

                var result = await conn.ExecuteAsync(query, new
                {
                    UserPhoto = photoBytes,
                    UserId = userId
                });

                return result > 0;
            }
        }

        public async Task<List<Tag>> GetUserTags(int userId)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                string query = @"
                    SELECT DISTINCT t.IdTag, t.NameTag
                    FROM Recipes_Tags rt
                    JOIN Tags t ON rt.IdTag = t.IdTag
                    JOIN Recipe r ON r.Id_Recipe = rt.IdRecipe
                    WHERE r.Id_User = @UserId";

                var tags = await conn.QueryAsync<Tag>(query, new { UserId = userId });
                return tags.ToList();
            }
        }

    }
}

