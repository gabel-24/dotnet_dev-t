using WA1.DTOs;
using WA1.Models;

namespace WA1.Repositories
{
    public interface IUserRepository
    {
        Task<User> AddUser(User newUser);
        Task<List<User>> AddUsers(List<User> newUsers);
        Task<User?> GetUserById(int id);
        Task<List<User>> GetAllUsers();
        Task<bool> DeleteUser(int id);
        Task<bool> UpdateUser(User updatedUser);
        Task<User?> UserExists(string username);
        Task<(List<User> Users, int TotalCount)> GetFiltered(UserQueryParams queryParams);
    }
}
