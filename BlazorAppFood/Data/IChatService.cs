using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorAppFood.Models;

namespace BlazorAppFood.Data
{
    interface IChatService
    {
        Task<int> CreateComment(int idRecipe, int idUser, string message);

        Task<List<Comment>> LoadRecipeComments(int idRecipe);
    }
}
