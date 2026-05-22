using BlazorAppFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppFood.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserInfo(string emailAddress);
        Task<bool> FollowUser(int currentUserId, int userIdToFollow);
        Task<bool> IsFollowing(int currentUserId, int userIdToCheck);
        Task<bool> UnfollowUser(int currentUserId, int userIdToUnfollow);
        Task<int> GetFollowerCount(int userId);
        Task<List<User>> GetFollowedUsers(int userId);
        Task<List<Recipe>> GetFollowingRecipes(string emailAddress);
        Task<IEnumerable<User>> UsersList();
        Task<User> GetUserByEmail(string email);
        Task<bool> UpdateUser(User user);
        Task<User> GetUserById(int idUser);
        Task<bool> UploadPhoto(int userId, byte[] photoBytes);
        Task<List<Tag>> GetUserTags(int userId);
    }
}
