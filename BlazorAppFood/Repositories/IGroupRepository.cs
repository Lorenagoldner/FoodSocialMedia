using BlazorAppFood.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorAppFood.Repositories
{
    public interface IGroupRepository
    {
        Task<int> CreateGroup(string name, int userId);
        Task DeleteGroup(int groupId, int requestingUserId);
        Task SetUserAdminStatus(int userId, int groupId, bool isAdmin);
        Task<bool> IsUserAdmin(int userId, int groupId);
        Task<bool> IsUserInGroup(int userId, int groupId);
        Task AddUserToGroup(int userId, int groupId);
        Task<bool> RemoveUserFromGroup(int userId, int groupId);
        Task<Group> GetGroupById(int groupId);
        Task<List<Group>> GetUserGroups(int userId);
        Task<List<Recipe>> GetGroupRecipes(int userId);
        Task<List<GroupMember>> GetGroupMembers(int groupId);
        Task<List<GroupChatMessages>> GetChatMessages(int groupId);
        Task<List<Group>> GetAllGroups();
        Task<List<Group>> SearchGroupsByName(string name);
        Task<List<Group>> SearchGroupsByNameStartingWith(string searchQuery);
        Task<(string CreatorName, int FollowersCount)> GetGroupMetadata(int groupId);
        Task DeleteChatMessage(int messageId, string requestingUsername, bool isAdmin);

    }

}
