using BlazorAppFood.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorAppFood.Repositories
{
    public interface IRecipeRepository
    {
        Task<bool> CreateRecipe(Recipe recipe);
        Task<IEnumerable<Recipe>> RecipeList();
        Task<List<Recipe>> RecipesList(int userId);
        Task<IEnumerable<Ingredient>> IngredientList();
        Task<Recipe> Recipe_GetOne(int id);
        Task<bool> UpdateRecipe(Recipe recipe);
        Task<bool> DeleteRecipe(int id);

        Task<int> RatingRecipe(int idRecipe, int idUser, int valueRating);
        Task<Recipe> GetRecipeInfo(int idRecipe);

        Task<bool> AddFavorite(int idRecipe, int idUser);
        Task<bool> CheckFavorite(int idRecipe, int idUser);
        Task<List<Recipe>> GetFavoriteRecipes(int userId);
        Task<int> GetFavoriteCount(int recipeId);
        Task<IEnumerable<Recipe>> GetRecipesByUserId(int userId);
        Task<List<Category>> GetAllCategoriesAsync();
        Task<List<Tag>> GetAllTagsAsync();
        Task<List<Recipe>> GetRecipesByTag(int tagId);
        Task<List<Recipe>> GetAllRecipes();
        Task<List<Tag>> GetTagsForRecipe(int recipeId);
        Task<List<Recipe>> GetMostFavoritedRecipes();
        Task<int?> GetUserRating(int idRecipe, int idUser);
    }
}