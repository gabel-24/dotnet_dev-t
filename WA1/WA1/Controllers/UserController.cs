using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WA1.DTOs;
using WA1.Services;

namespace WA1.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> AddUser(AddUserDto newUser)
        {
            var user = await _userService.AddUser(newUser);

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> UserLogin(LoginUserDto loginUser)
        {
            var user = await _userService.UserLogin(loginUser);

            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(user);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);

            if(user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost("register/bulk")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AddUsers(List<AddUserDto> newUsers)
        {
            var addedUsers = await _userService.AddUsers(newUsers);
            return Ok(addedUsers);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery]UserQueryParams queryParams)
        {
            var users = await _userService.GetAllUsers(queryParams);
            return Ok(users);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            bool success = await _userService.DeleteUser(id);

            if (!success)
            {
                return NotFound();
            }
            else
            {
                return NoContent();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, AddUserDto updatedUser)
        {
            bool success = await _userService.UpdateUser(id, updatedUser);

            if (!success)
            {
                return NotFound();
            }
            else
            {
                return NoContent();
            }
        }
    }
}
