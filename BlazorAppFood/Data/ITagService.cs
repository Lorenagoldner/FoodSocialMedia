using Azure;
using BlazorAppFood.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorAppFood.Data
{
    public interface ITagService
    {
        Task<List<CategoryWithTags>> GetCategoriesWithTags();
        Task<List<Tag>> GetTagsByIds(IEnumerable<int> tagIds);
    }
}
