using Azure;
using BlazorAppFood.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorAppFood.Repositories
{
    public interface ITagRepository
    {
        Task<List<CategoryWithTags>> GetCategoriesWithTags();
        Task<List<Tag>> GetTagsByIds(IEnumerable<int> tagIds);
    }
}
