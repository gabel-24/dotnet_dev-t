using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WA1.Data;
using WA1.DTOs;
using WA1.Models;
using WA1.Repositories;
using WA1.Services;

namespace WA1.Services
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _repository;
        private readonly TokenService _tokenService;

        private readonly PasswordHasher<User> _passwordHasher = new();

        public UserService(IUserRepository repository, TokenService tokenService)
        {
            _repository = repository;
            _tokenService = tokenService;
        }

        public static UserDto ToDto(User u) => new UserDto
        {
            Id = u.id,
            Username = u.Username,
            Role = u.Role
        };

        public async Task<UserDto> AddUser(AddUserDto newUser)
        {
            var user = new User
            {
                Username = newUser.Username,
                Role = newUser.Role ?? "User"
            };

            user.PasswordHarsh = _passwordHasher.HashPassword(user, newUser.Password);

           var created = await _repository.AddUser(user);

            return new UserDto {Id = created.id, Username = created.Username, Role = created.Role };
        }
        public async Task<List<UserDto>> AddUsers(List<AddUserDto> newUsers)
        {
            var users = newUsers.Select(u =>
            {
                var user = new User
                {
                    Username = u.Username,
                    Role = u.Role ?? "User"
                };
                user.PasswordHarsh = _passwordHasher.HashPassword(user, u.Password);

                return user;
            }).ToList();

            var created = await _repository.AddUsers(users);

            return created.Select(c => new UserDto
            {
                Id = c.id,
                Username = c.Username,
                Role = c.Role
            }).ToList();
        }
        public async Task<UserDto?> GetUserById(int id)
        {
            var user = await _repository.GetUserById(id);

            return user == null ? null : ToDto(user);
        }

        public async Task<PagedResult<UserDto>> GetAllUsers(UserQueryParams queryParams)
        {
            var (users, totalCount) = await _repository.GetFiltered(queryParams);

            return new PagedResult<UserDto>
            {
                Items = users.Select(ToDto).ToList(),
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }
        public async Task<bool> DeleteUser(int id)
        {
            return await _repository.DeleteUser(id);
        }
        public async Task<bool> UpdateUser(int id, AddUserDto updatedUser)
        {
            var user = await _repository.GetUserById(updatedUser.Id);

            if (user == null)
            {
                return false;
            }

            user.Username = updatedUser.Username;
            user.Role = updatedUser.Role ?? user.Role; // keep existing role if none provided

            if (!string.IsNullOrWhiteSpace(updatedUser.Password))
            {
                user.PasswordHarsh = _passwordHasher.HashPassword(user, updatedUser.Password); // hash it, don't store raw
            }

            var success = await _repository.UpdateUser(user);

            if (!success)
            {
                return false;
            }

            return true;
        }

        public async Task<AuthResponseDto?> UserLogin(LoginUserDto userdto)
        {
            var user = await _repository.UserExists(userdto.Username);

            if(user == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHarsh, userdto.Password);

            if (result == PasswordVerificationResult.Failed) return null;

            var token = _tokenService.GenerateToken(user);


            return new AuthResponseDto {
                Token = token,
                User = new UserDto { Id = user.id, Username = user.Username }
            };
        }

        

        
    }
}
