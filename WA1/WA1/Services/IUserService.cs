using WA1.DTOs;

namespace WA1.Services
{
    public interface IUserService
    {
        Task<UserDto> AddUser(AddUserDto newUser);
        Task<List<UserDto>> AddUsers(List<AddUserDto> newUsers);        
        Task<UserDto?> GetUserById(int id);
        Task<PagedResult<UserDto>> GetAllUsers(UserQueryParams queryParams);
        Task<bool> DeleteUser(int id);
        Task<bool> UpdateUser(int id, AddUserDto updatedUser);
        Task<AuthResponseDto?> UserLogin(LoginUserDto userdto);
        
    }
}
