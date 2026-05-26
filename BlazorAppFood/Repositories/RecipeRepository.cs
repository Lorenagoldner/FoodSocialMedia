using BlazorAppFood.Configuration;
using BlazorAppFood.Data;
using BlazorAppFood.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BlazorAppFood.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        //Database Connection
        private readonly SqlConnectionConfiguration _configuration;
        private readonly INotificationRepository _notificationRepository;

        public RecipeRepository(
            SqlConnectionConfiguration configuration,
            INotificationRepository notificationRepository)
        {
            _configuration = configuration;
            _notificationRepository = notificationRepository;
        }

        //Add 
        public async Task<bool> CreateRecipe(Recipe recipe)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                await conn.OpenAsync();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("Id_User", recipe.Id_User, DbType.Int32);
                        parameters.Add("NameRecipe", recipe.NameRecipe, DbType.String);
                        parameters.Add("Prep_Time", recipe.Prep_Time, DbType.Int32);
                        parameters.Add("Cook_Time", recipe.Cook_Time, DbType.Int32);
                        parameters.Add("Preparation", recipe.Preparation, DbType.String);
                        parameters.Add("Image", recipe.Image, DbType.Binary);
                        parameters.Add("Recipe_Id_Out", 0, DbType.Int32, direction: ParameterDirection.Output);

                        // Call stored procedure that inserts recipe and post
                        await conn.ExecuteAsync("spRecipe_Insert", parameters, transaction, commandType: CommandType.StoredProcedure);

                        int recp_id = parameters.Get<int>("Recipe_Id_Out");
                        recipe.Id_Recipe = recp_id;

                        // Loop for each ingredient
                        foreach (var ingredient in recipe.Ingredients)
                        {
                            var existingIngredientId = await conn.ExecuteScalarAsync<int?>(
                                "SELECT Id_Ingredient FROM Ingredients WHERE LOWER(Name) = LOWER(@Name)",
                                new { Name = ingredient.Name },
                                transaction
                                );

                            int ingredientId;

                            if (existingIngredientId.HasValue)
                            {
                                ingredientId = existingIngredientId.Value;
                            }
                            else
                            {
                                // Insert new ingredient
                                string insertIngredientSql = @"
                                INSERT INTO Ingredients (Name)
                                VALUES (@Name);
                                SELECT CAST(SCOPE_IDENTITY() as int);
                            ";

                                ingredientId = await conn.ExecuteScalarAsync<int>(insertIngredientSql, new { Name = ingredient.Name }, transaction);
                            }

                            // Insert relation in Recipes_Ingredients
                            string insertRelationSql = @"
                            INSERT INTO Recipes_Ingredients (Id_Recipe, Id_Ingredient)
                            VALUES (@IdRecipe, @IdIngredient);
                        ";

                            await conn.ExecuteAsync(insertRelationSql, new
                            {
                                IdRecipe = recp_id,
                                IdIngredient = ingredientId
                            }, transaction);
                        }

                        foreach (var tagId in recipe.TagIds)
                        {
                            // Insert relation in Recipes_Tags
                            string insertTagRelationSql = @"
                                INSERT INTO Recipes_Tags (IdRecipe, IdTag)
                                VALUES (@IdRecipe, @IdTag);
                            ";

                            await conn.ExecuteAsync(insertTagRelationSql, new
                            {
                                IdRecipe = recp_id,
                                IdTag = tagId
                            }, transaction);
                        }

                        await transaction.CommitAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine($"Erro ao criar receita: {ex.Message}");
                        return false;
                    }
                }
            }
        }



        //Show Recipe
        public async Task<IEnumerable<Recipe>> RecipeList()
        {
            IEnumerable<Recipe> recipe;

            using (var conn = new SqlConnection(_configuration._value))
            {
                recipe = await conn.QueryAsync<Recipe>("spRecipe_GetAll", commandType: CommandType.StoredProcedure);
            }
            return recipe;
        }

        // Show Ingredients

        public async Task<IEnumerable<Ingredient>> IngredientList()
        {
            IEnumerable<Ingredient> ingredients;
            using (var conn = new SqlConnection(_configuration._value))
            {
                ingredients = await conn.QueryAsync<Ingredient>("spIngredients_GetAll", commandType: CommandType.StoredProcedure);
            }

            return ingredients;
        }

        public async Task<List<Recipe>> RecipesList(int idUser)
        {
            //List<Recipe> recipe;

            using (var conn = new SqlConnection(_configuration._value))
            {
                string sQuery = @"SELECT * FROM Recipe Where Id_User = @idUser";
                return (await conn.QueryAsync<Recipe>(sQuery, new { IdUser = idUser }, commandType: CommandType.Text)).ToList();
            }
            //return recipe;
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByUserId(int userId)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                string query = "SELECT * FROM Recipe WHERE Id_User = @UserId";

                // Fetch the recipes where the user ID matches
                var recipes = await conn.QueryAsync<Recipe>(query, new { UserId = userId });

                return recipes;
            }
        }

        //Get one recipe based on Id_Recipe
        //public async Task<Recipe> Recipe_GetOne(int id)
        //{
        //    Recipe recipe = new Recipe();
        //    var parameters = new DynamicParameters();
        //    parameters.Add("Id_Recipe", id, DbType.Int32);
        //    using (var conn = new SqlConnection(_configuration._value))
        //    {
        //        recipe = await conn.QueryFirstOrDefaultAsync<Recipe>("spGetRecipe_GetOne", parameters, commandType: CommandType.StoredProcedure);
        //    }
        //    return recipe;
        //}

        public async Task<Recipe> Recipe_GetOne(int id)
        {
            Recipe recipe = new Recipe();
            var parameters = new DynamicParameters();
            parameters.Add("Id_Recipe", id, DbType.Int32);

            using (var conn = new SqlConnection(_configuration._value))
            {
                // Execute the stored procedure to fetch the recipe, ingredients, and tags
                var result = await conn.QueryAsync<Recipe, Ingredient, Tag, Recipe>(
                    "spGetRecipe_GetOne",
                    (recipeData, ingredientData, tagData) =>
                    {
                        // Assign basic recipe data
                        recipe = recipeData;

                        if (ingredientData != null)
                        {
                            recipe.Ingredients.Add(ingredientData);
                        }

                        if (tagData != null)
                        {
                            recipe.Tags.Add(tagData);
                        }

                        return recipe;
                    },
                    parameters,
                    splitOn: "Id_Ingredient, IdTag",
                    commandType: CommandType.StoredProcedure
                );
            }

            return recipe;
        }

        //Update
        //public async Task<bool> UpdateRecipe(Recipe recipe)
        //{
        //    using (var conn = new SqlConnection(_configuration._value))
        //    {
        //        var parameters = new DynamicParameters();
        //        parameters.Add("Id_Recipe", recipe.Id_Recipe, DbType.Int32);
        //        parameters.Add("NameRecipe", recipe.NameRecipe, DbType.String);
        //        parameters.Add("Prep_Time", recipe.Prep_Time, DbType.String);
        //        parameters.Add("Cook_Time", recipe.Cook_Time, DbType.String);
        //        parameters.Add("Preparation", recipe.Preparation, DbType.String);
        //        parameters.Add("Image", recipe.Image, DbType.Byte);
        //        const string query = @"UPDATE Recipe SET NameRecipe = @NameRecipe, Prep_Time = @Prep_Time, Cook_Time = @Cook_Time, Preparation = @Preparation, [Image] = @Image WHERE (Id_Recipe = @Id_Recipe)";
        //        await conn.ExecuteAsync(query, new {recipe.Id_Recipe, recipe.NameRecipe, recipe.Prep_Time, recipe.Cook_Time, recipe.Preparation, recipe.Image }, commandType: CommandType.Text);
        //        //await conn.ExecuteAsync("spRecipe_Update", parameters, commandType: CommandType.StoredProcedure);
        //    }

        //    return true;
        //}

        public async Task<bool> UpdateRecipe(Recipe recipe)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                await conn.OpenAsync();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Update basic Recipe fields
                        // [Image] = COALESCE(@Image, [Image]) -> se @Image for null, mantem a imagem existente
                        const string updateRecipeQuery = @"
                    UPDATE Recipe
                    SET NameRecipe = @NameRecipe,
                        Prep_Time = @Prep_Time,
                        Cook_Time = @Cook_Time,
                        Preparation = @Preparation,
                        [Image] = COALESCE(@Image, [Image])
                    WHERE Id_Recipe = @Id_Recipe";

                        await conn.ExecuteAsync(updateRecipeQuery, new
                        {
                            recipe.Id_Recipe,
                            recipe.NameRecipe,
                            recipe.Prep_Time,
                            recipe.Cook_Time,
                            recipe.Preparation,
                            recipe.Image
                        }, transaction, commandType: CommandType.Text);

                        // Update Ingredients
                        if (recipe.Ingredients != null)
                        {
                            // Delete old ingredients
                            await conn.ExecuteAsync("DELETE FROM Recipes_Ingredients WHERE Id_Recipe = @RecipeId", new { RecipeId = recipe.Id_Recipe }, transaction);

                            // Insert new ingredients
                            foreach (var ingredient in recipe.Ingredients)
                            {
                                await conn.ExecuteAsync(@"
                            INSERT INTO Recipes_Ingredients (Id_Recipe, Id_Ingredient) 
                            VALUES (@RecipeId, @IngredientId)",
                                    new
                                    {
                                        RecipeId = recipe.Id_Recipe,
                                        IngredientId = ingredient.Id_Ingredient
                                    }, transaction);
                            }
                        }

                        // Update Tags
                        if (recipe.Tags != null)
                        {
                            // Delete old tags
                            await conn.ExecuteAsync("DELETE FROM Recipes_Tags WHERE IdRecipe = @RecipeId", new { RecipeId = recipe.Id_Recipe }, transaction);

                            // Insert new tags
                            foreach (var tag in recipe.Tags)
                            {
                                await conn.ExecuteAsync(@"
                            INSERT INTO Recipes_Tags (IdRecipe, IdTag) 
                            VALUES (@RecipeId, @TagId)",
                                    new
                                    {
                                        RecipeId = recipe.Id_Recipe,
                                        TagId = tag.IdTag
                                    }, transaction);
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }

            return true;
        }

        //Delete
        public async Task<bool> DeleteRecipe(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Id_Recipe", id, DbType.Int32);
            using (var conn = new SqlConnection(_configuration._value))
            {
                await conn.ExecuteAsync("spRecipe_Delete", parameters, commandType: CommandType.StoredProcedure);
            }
            return true;
        }

        //Rating Recipe

        public async Task<int> RatingRecipe(int idRecipe, int idUser, int valueRating)
        {
            
            using (var conn = new SqlConnection(_configuration._value))
            {
                string sql = @"IF NOT EXISTS (SELECT * FROM Ratings WHERE Id_Recipe = @idRecipe AND Id_User = @idUser) 
                                BEGIN INSERT INTO Ratings(Id_Recipe, Id_User, RatingValue) VALUES(@idRecipe, @idUser, @valueRating) END 
                                ELSE BEGIN UPDATE Ratings SET RatingValue = @valueRating WHERE Id_Recipe = @idRecipe AND Id_User = @idUser END; 
                                UPDATE  Recipe SET AverageRating = ( SELECT CAST(AVG(RatingValue) AS DECIMAL(2,1)) FROM Ratings WHERE Id_Recipe = @idRecipe)
                                WHERE Id_Recipe = @idRecipe;";

                //await conn.QueryMultipleAsync(sql,
                //                              new
                //                              {
                //                                  IdUser = idUser,
                //                                  IdRecipe = idRecipe,
                //                                  ValueRating = valueRating
                //                              });
                //return 1;
                await conn.ExecuteAsync(sql, new
                {
                    idUser,
                    idRecipe,
                    valueRating
                });

                var recipeInfo = await conn.QuerySingleOrDefaultAsync<Recipe>(
                    @"SELECT r.Id_User, r.NameRecipe
              FROM Recipe r
              WHERE r.Id_Recipe = @IdRecipe",
                    new { IdRecipe = idRecipe });

                var actorName = await conn.ExecuteScalarAsync<string>(
                    @"SELECT Username FROM Users WHERE Id_User = @IdUser",
                    new { IdUser = idUser });

                if (recipeInfo != null && recipeInfo.Id_User != idUser)
                {
                    await _notificationRepository.CreateNotification(new Notification
                    {
                        RecipientUserId = recipeInfo.Id_User,
                        ActorUserId = idUser,
                        Type = NotificationType.Rating,
                        Message = $"{actorName} avaliou a tua receita {recipeInfo.NameRecipe}",
                        RelatedEntityId = idRecipe,
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    });
                }

                return 1;
            }
        }

        public async Task<int?> GetUserRating(int idRecipe, int idUser)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                string query = "SELECT RatingValue FROM Ratings WHERE Id_Recipe = @IdRecipe AND Id_User = @IdUser";
                return await conn.ExecuteScalarAsync<int?>(query, new { IdRecipe = idRecipe, IdUser = idUser });
            }
        }

        // Select Recipe

        public async Task<Recipe> GetRecipeInfo(int idRecipe)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                string recipeQuery = @"
            SELECT Recipe.*, Users.Username, Users.UserPhoto
            FROM Recipe
            INNER JOIN Users ON Recipe.Id_User = Users.Id_User
            WHERE Id_Recipe = @IdRecipe";

                var recipe = await conn.QuerySingleAsync<Recipe>(recipeQuery, new { IdRecipe = idRecipe });

                // Load Ingredients
                string ingredientsQuery = @"
                    SELECT i.Id_Ingredient, i.Name
                    FROM Recipes_Ingredients ri
                    JOIN Ingredients i ON ri.Id_Ingredient = i.Id_Ingredient
                    WHERE ri.Id_Recipe = @IdRecipe";

                var ingredients = await conn.QueryAsync<Ingredient>(ingredientsQuery, new { IdRecipe = idRecipe });
                recipe.Ingredients = ingredients.ToList();

                // Load Tags
                string tagsQuery = @"
                    SELECT t.IdTag, t.NameTag
                    FROM Recipes_Tags rt
                    JOIN Tags t ON rt.IdTag = t.IdTag
                    WHERE rt.IdRecipe = @IdRecipe";

                var tags = await conn.QueryAsync<Tag>(tagsQuery, new { IdRecipe = idRecipe });
                recipe.Tags = tags.ToList();

                return recipe;
            }
        }

        public async Task<bool> AddFavorite(int idRecipe, int idUser)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                string checkQuery = @"SELECT COUNT(*) 
                              FROM Favorites 
                              WHERE IdRecipe = @IdRecipe AND IdUser = @IdUser";

                int alreadyFavorite = await conn.ExecuteScalarAsync<int>(
                    checkQuery,
                    new { IdRecipe = idRecipe, IdUser = idUser });

                string myQuery = @"IF NOT EXISTS (SELECT * FROM Favorites WHERE IdRecipe = @IdRecipe AND IdUser = @idUser) 
                                 INSERT INTO Favorites(IdUser, IdRecipe) VALUES(@idUser, @idRecipe)
                                 ELSE DELETE FROM Favorites WHERE IdUser = @idUser AND IdRecipe = @idRecipe";

                // Use ExecuteAsync instead of QuerySingleAsync as we're not expecting a result set
                await conn.ExecuteAsync(myQuery,
                                        new
                                        {
                                            IdRecipe = idRecipe,
                                            IdUser = idUser
                                        });

                if (alreadyFavorite == 0)
                {
                    var recipeInfo = await conn.QuerySingleOrDefaultAsync<Recipe>(
                        @"SELECT r.Id_User, r.NameRecipe
                  FROM Recipe r
                  WHERE r.Id_Recipe = @IdRecipe",
                        new { IdRecipe = idRecipe });

                    var actorName = await conn.ExecuteScalarAsync<string>(
                        @"SELECT Username FROM Users WHERE Id_User = @IdUser",
                        new { IdUser = idUser });

                    if (recipeInfo != null && recipeInfo.Id_User != idUser)
                    {
                        await _notificationRepository.CreateNotification(new Notification
                        {
                            RecipientUserId = recipeInfo.Id_User,
                            ActorUserId = idUser,
                            Type = NotificationType.Favorite,
                            Message = $"{actorName} favoritou a tua receita {recipeInfo.NameRecipe}",
                            RelatedEntityId = idRecipe,
                            IsRead = false,
                            CreatedAt = DateTime.Now
                        });
                    }
                }
            }

            return true;
        }

        public async Task<bool> CheckFavorite(int idRecipe, int idUser)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {

                string query = "SELECT COUNT(*) FROM Favorites WHERE IdUser = @IdUser AND IdRecipe = @IdRecipe";
                var count = await conn.ExecuteScalarAsync<int>(query, new { IdUser = idUser, IdRecipe = idRecipe });
                return count > 0;
            }         
        }

        public async Task<List<Recipe>> GetFavoriteRecipes(int userId)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                string query = @"
                    SELECT r.*
                    FROM Favorites f
                    JOIN Recipe r ON f.IdRecipe = r.Id_Recipe
                    WHERE f.IdUser = @UserId";

                var recipes = (await conn.QueryAsync<Recipe>(query, new { UserId = userId })).ToList();

                // Optionally load tags, images, etc., for each recipe
                foreach (var recipe in recipes)
                {
                    recipe.Tags = await GetTagsForRecipe(recipe.Id_Recipe);
                }

                return recipes;
            }
        }

        public async Task<int> GetFavoriteCount(int recipeId)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                string query = "SELECT COUNT(*) FROM Favorites WHERE IdRecipe = @RecipeId";
                return await conn.ExecuteScalarAsync<int>(query, new { RecipeId = recipeId });
            }
        }



        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            var categories = new List<Category>();

            using (var connection = new SqlConnection(_configuration._value))
            {
                await connection.OpenAsync();

                var command = new SqlCommand("SELECT IdCategory, Description FROM Categories", connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        categories.Add(new Category
                        {
                            IdCategory = reader.GetInt32(0),
                            Description = reader.GetString(1)
                        });
                    }
                }
            }

            return categories;
        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            var tags = new List<Tag>();

            using (var connection = new SqlConnection(_configuration._value))
            {
                await connection.OpenAsync();

                var command = new SqlCommand("SELECT IdTag, NameTag, IdCategory FROM Tags", connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tags.Add(new Tag
                        {
                            IdTag = reader.GetInt32(0),
                            NameTag = reader.GetString(1),
                            IdCategory = reader.GetInt32(2)
                        });
                    }
                }
            }

            return tags;
        }

        public async Task<List<Recipe>> GetRecipesByTag(int tagId)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                string query = @"
                    SELECT r.*, u.Username, u.UserPhoto
                    FROM Recipe r
                    INNER JOIN Recipes_Tags rt ON r.Id_Recipe = rt.IdRecipe
                    INNER JOIN Users u ON r.Id_User = u.Id_User
                    WHERE rt.IdTag = @TagId";

                var recipes = await conn.QueryAsync<Recipe>(query, new { TagId = tagId });
                return recipes.ToList();
            }
        }

        public async Task<List<Recipe>> GetAllRecipes()
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                string query = @"
                    SELECT r.*, u.Username, u.UserPhoto
                    FROM Recipe r
                    INNER JOIN Users u ON r.Id_User = u.Id_User";

                var recipes = await conn.QueryAsync<Recipe>(query);
                return recipes.ToList();
            }
        }

        public async Task<List<Tag>> GetTagsForRecipe(int recipeId)
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                string query = @"
                    SELECT t.IdTag, t.NameTag
                    FROM Recipes_Tags rt
                    JOIN Tags t ON rt.IdTag = t.IdTag
                    WHERE rt.IdRecipe = @RecipeId";

                var tags = await conn.QueryAsync<Tag>(query, new { RecipeId = recipeId });
                return tags.ToList();
            }
        }

        public async Task<List<Recipe>> GetMostFavoritedRecipes()
        {
            using (var conn = new SqlConnection(_configuration._value))
            {
                string query = @"
                SELECT TOP 3 
                    r.Id_Recipe,
                    r.Id_User,
                    r.NameRecipe,
                    r.Prep_Time,
                    r.Cook_Time,
                    CAST(r.Preparation AS NVARCHAR(MAX)) AS Preparation,
                    r.Image,
                    r.Video,
                    r.AverageRating,
                    r.PublishedDate,
                COUNT(f.IdRecipe) AS FavoriteCount
                FROM Recipe r
                LEFT JOIN Favorites f ON r.Id_Recipe = f.IdRecipe
                GROUP BY 
                    r.Id_Recipe,
                    r.Id_User,
                    r.NameRecipe,
                    r.Prep_Time,
                    r.Cook_Time,
                    CAST(r.Preparation AS NVARCHAR(MAX)),
                    r.Image,
                    r.Video,
                    r.AverageRating,
                    r.PublishedDate
                ORDER BY FavoriteCount DESC";

                var recipes = await conn.QueryAsync<Recipe>(query);

                return recipes.ToList();
            }
        }
    }
}
