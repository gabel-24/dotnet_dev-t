using Microsoft.EntityFrameworkCore;
using WA1.Data;
using WA1.DTOs;
using WA1.Models;

namespace WA1.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }       

        public async Task<User> AddUser(User newUser)
        {
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;

        }
        public async Task<List<User>> AddUsers(List<User> newUsers)
        {
            _context.Users.AddRange(newUsers);
            await _context.SaveChangesAsync();

            return newUsers.ToList();
        }
        public async Task<User?> GetUserById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.id == id);
        }
        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<bool> DeleteUser(int id)
        {
            var user = _context.Users.Find(id);

            if(user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateUser(User updatedUser)
        {
            var user = _context.Users.Find(updatedUser.id);

            if( user == null)
            {
                return false;
            }

            user.Username = updatedUser.Username;
            user.PasswordHarsh = updatedUser.PasswordHarsh;
            user.Role = updatedUser.Role;

            await _context.SaveChangesAsync();
            return true;

        }
        public async Task<User?> UserExists(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<(List<User> Users, int TotalCount)> GetFiltered(UserQueryParams queryParams)
        {
            var query = _context.Users.AsQueryable();

            //filter
            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                var search = queryParams.Search;
                query = query.Where(u => u.Username.ToLower().Contains(search));
            }

            //sorting
            query = queryParams.SortBy.ToLower() switch
            {
                "username" => queryParams.Descending ? query.OrderByDescending(u => u.Username) : query.OrderBy(u => u.Username),
                "role" => queryParams.Descending ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role),
                _ => queryParams.Descending ? query.OrderByDescending(u => u.id) : query.OrderBy(u => u.id)
            };

            int totalCount = await query.CountAsync();

            var users = await query
                                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                                .Take(queryParams.PageSize)
                                .ToListAsync();

            return (users, totalCount);
        }

    }
}
