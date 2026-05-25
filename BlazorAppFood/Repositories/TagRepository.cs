using Azure;
using BlazorAppFood.Configuration;
using BlazorAppFood.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorAppFood.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly SqlConnectionConfiguration _configuration;
        public TagRepository(SqlConnectionConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<CategoryWithTags>> GetCategoriesWithTags()
        {
            var categories = new List<CategoryWithTags>();

            using (var connection = new SqlConnection(_configuration._value))
            {
                await connection.OpenAsync();

                // First, get categories
                var categoryCmd = new SqlCommand("SELECT IdCategory, Description FROM Categories", connection);
                using (var reader = await categoryCmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        categories.Add(new CategoryWithTags
                        {
                            IdCategory = reader.GetInt32(0),
                            Description = reader.GetString(1),
                            Tags = new List<Tag>()
                        });
                    }
                }

                // Then, get tags for each category
                foreach (var category in categories)
                {
                    var tagCmd = new SqlCommand("SELECT IdTag, NameTag FROM Tags WHERE IdCategory = @CategoryId", connection);
                    tagCmd.Parameters.AddWithValue("@CategoryId", category.IdCategory);

                    using (var reader = await tagCmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            category.Tags.Add(new Tag
                            {
                                IdTag = reader.GetInt32(0),
                                NameTag = reader.GetString(1),
                                IdCategory = category.IdCategory
                            });
                        }
                    }
                }
            }

            return categories;
        }

        public async Task<List<Tag>> GetTagsByIds(IEnumerable<int> tagIds)
        {
            var tags = new List<Tag>();

            using (var connection = new SqlConnection(_configuration._value))
            {
                await connection.OpenAsync();

                foreach (var id in tagIds)
                {
                    var cmd = new SqlCommand("SELECT IdTag, NameTag, IdCategory FROM Tags WHERE IdTag = @Id", connection);
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
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
            }

            return tags;
        }
    }
}

