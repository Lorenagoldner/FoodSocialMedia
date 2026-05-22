using BlazorAppFood.Models;
using Dapper;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BlazorAppFood.Configuration;

namespace BlazorAppFood.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly SqlConnectionConfiguration _configuration;

        public GroupRepository(SqlConnectionConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> CreateGroup(string name, int userId)
        {
            using var conn = new SqlConnection(_configuration._value);

            // Insert group and get its new ID
            string insertGroup = "INSERT INTO Groups (Name) VALUES (@Name); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            int groupId = await conn.ExecuteScalarAsync<int>(insertGroup, new { Name = name });

            // Add creator to group
            string insertMembership = "INSERT INTO Users_Groups (Id_User, Id_Group, IsAdmin) VALUES (@UserId, @GroupId, 1)";
            await conn.ExecuteAsync(insertMembership, new { UserId = userId, GroupId = groupId });

            return groupId;
        }

        public async Task DeleteGroup(int groupId, int requestingUserId)
        {
            using var conn = new SqlConnection(_configuration._value);

            bool isAdmin = await IsUserAdmin(requestingUserId, groupId);
            if (!isAdmin)
                throw new UnauthorizedAccessException("Only admins can delete the group.");

            await conn.ExecuteAsync("DELETE FROM Users_Groups WHERE Id_Group = @GroupId", new { GroupId = groupId });
            await conn.ExecuteAsync("DELETE FROM Groups WHERE Id_Group = @GroupId", new { GroupId = groupId });
        }

        public async Task AddUserToGroup(int userId, int groupId)
        {
            using var conn = new SqlConnection(_configuration._value);

            // Check if the user is already in the group
            bool userInGroup = await IsUserInGroup(userId, groupId);

            if (userInGroup)
            {
                // If the user is already in the group, we can just return without adding them again
                return;
            }

            // If the user is not in the group, add them
            string query = "INSERT INTO Users_Groups (Id_User, Id_Group) VALUES (@UserId, @GroupId)";
            await conn.ExecuteAsync(query, new { UserId = userId, GroupId = groupId });
        }

        public async Task<bool> RemoveUserFromGroup(int userId, int groupId)
        {
            using var conn = new SqlConnection(_configuration._value);
            string query = "DELETE FROM Users_Groups WHERE Id_User = @UserId AND Id_Group = @GroupId";

            var rowsAffected = await conn.ExecuteAsync(query, new { UserId = userId, GroupId = groupId });

            return rowsAffected > 0; // Returns true if the user was successfully removed
        }

        public async Task SetUserAdminStatus(int userId, int groupId, bool isAdmin)
        {
            using var conn = new SqlConnection(_configuration._value);
            string query = "UPDATE Users_Groups SET IsAdmin = @IsAdmin WHERE Id_User = @UserId AND Id_Group = @GroupId";
            await conn.ExecuteAsync(query, new { UserId = userId, GroupId = groupId, IsAdmin = isAdmin });
        }

        public async Task<bool> IsUserAdmin(int userId, int groupId)
        {
            using var conn = new SqlConnection(_configuration._value);
            string query = "SELECT IsAdmin FROM Users_Groups WHERE Id_User = @UserId AND Id_Group = @GroupId";
            return await conn.ExecuteScalarAsync<bool>(query, new { UserId = userId, GroupId = groupId });
        }

        public async Task<bool> IsUserInGroup(int userId, int groupId)
        {
            using var conn = new SqlConnection(_configuration._value);

            // Query to check if the user is already in the group
            string query = "SELECT COUNT(1) FROM Users_Groups WHERE Id_User = @UserId AND Id_Group = @GroupId";
            int userInGroupCount = await conn.ExecuteScalarAsync<int>(query, new { UserId = userId, GroupId = groupId });

            // If the count is greater than 0, the user is already in the group
            return userInGroupCount > 0;
        }

        public async Task<Group> GetGroupById(int groupId)
        {
            using var conn = new SqlConnection(_configuration._value);
            string query = "SELECT Id_Group, Name, Description FROM Groups WHERE Id_Group = @GroupId";
            return await conn.QueryFirstOrDefaultAsync<Group>(query, new { GroupId = groupId });
        }

        public async Task<List<Group>> GetUserGroups(int userId)
        {
            using var conn = new SqlConnection(_configuration._value);
            string query = @"SELECT g.Id_Group, g.Name 
                         FROM Groups g 
                         INNER JOIN Users_Groups ug ON g.Id_Group = ug.Id_Group 
                         WHERE ug.Id_User = @UserId";
            return (await conn.QueryAsync<Group>(query, new { UserId = userId })).ToList();
        }

        public async Task<List<Recipe>> GetGroupRecipes(int userId)
        {
            using var conn = new SqlConnection(_configuration._value);
            string query = @"
                        SELECT r.Id_Recipe, r.NameRecipe, r.Image, u.Username, u.UserPhoto, r.AverageRating
                        FROM Recipe r
                        JOIN Users u ON r.Id_User = u.Id_User
                        WHERE r.Id_User IN (
                            SELECT Id_User FROM Users_Groups 
                            WHERE Id_Group IN (
                                SELECT Id_Group FROM Users_Groups WHERE Id_User = @UserId
                            )
                        )";
            return (await conn.QueryAsync<Recipe>(query, new { UserId = userId })).ToList();
        }

        public async Task<List<GroupMember>> GetGroupMembers(int groupId)
        {
            using var conn = new SqlConnection(_configuration._value);

            string query = @"SELECT u.Id_User, u.Username, u.Email, u.UserPhoto, ug.IsAdmin 
                     FROM Users u
                     JOIN Users_Groups ug ON u.Id_User = ug.Id_User
                     WHERE ug.Id_Group = @GroupId";

            return (await conn.QueryAsync<GroupMember>(query, new { GroupId = groupId })).ToList();
        }

        public async Task<List<GroupChatMessages>> GetChatMessages(int groupId)
        {
            using var conn = new SqlConnection(_configuration._value);

            string query = @"SELECT GroupId AS Id_Group, Username, Message, Timestamp
                     FROM ChatMessages
                     WHERE GroupId = @GroupId
                     ORDER BY Timestamp";

            return (await conn.QueryAsync<GroupChatMessages>(query, new { GroupId = groupId })).ToList();
        }
        public async Task<List<Group>> GetAllGroups()
        {
            using var conn = new SqlConnection(_configuration._value);
            string query = "SELECT Id_Group, Name FROM Groups";
            return (await conn.QueryAsync<Group>(query)).ToList();
        }
        public async Task<(string CreatorName, int FollowersCount)> GetGroupMetadata(int groupId)
        {
            using var conn = new SqlConnection(_configuration._value);

            string query = @"
        SELECT 
            Creator.Username AS CreatorName,
            (SELECT COUNT(*) FROM Users_Groups WHERE Id_Group = @GroupId) AS FollowersCount
        FROM Users_Groups ug
        JOIN Users Creator ON Creator.Id_User = ug.Id_User
        WHERE ug.Id_Group = @GroupId AND ug.IsAdmin = 1";

            return await conn.QueryFirstOrDefaultAsync<(string CreatorName, int FollowersCount)>(
                query, new { GroupId = groupId });
        }

        public async Task<List<Group>> SearchGroupsByName(string name)
        {
            using var conn = new SqlConnection(_configuration._value);
            string query = "SELECT Id_Group, Name FROM Groups WHERE Name LIKE @Name";
            return (await conn.QueryAsync<Group>(query, new { Name = "%" + name + "%" })).ToList();
        }
       

        public async Task<List<Group>> SearchGroupsByNameStartingWith(string searchQuery)
        {
            using var conn = new SqlConnection(_configuration._value);
            string query = "SELECT Id_Group, Name FROM Groups WHERE Name LIKE @SearchQuery";

            return (await conn.QueryAsync<Group>(query, new { SearchQuery = searchQuery + "%" })).ToList();
        }




    }
}
