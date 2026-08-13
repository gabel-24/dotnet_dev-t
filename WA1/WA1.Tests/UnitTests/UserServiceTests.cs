using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using WA1.DTOs;
using WA1.Models;
using WA1.Repositories;
using WA1.Services;
using Xunit;

namespace WA1.Tests
{
    public class UserServiceTests
    {
        private static UserService CreateService(Mock<IUserRepository> mockRepo)
        {
            var mockConfig = new Mock<IConfiguration>();
            var tokenService = new TokenService(mockConfig.Object);
            return new UserService(mockRepo.Object, tokenService);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllUsers_ReturnsCorrectPagedResult()
        {
            var mockRepo = new Mock<IUserRepository>();

            var users = new List<User>
            {
                new User { id = 1, Username = "user1", Role = "student" },
                new User { id = 2, Username = "user2", Role = "student" },
                new User { id = 3, Username = "user3", Role = "lecturer" }
            };

            var queryParams = new UserQueryParams();

            mockRepo.Setup(repo => repo.GetFiltered(queryParams)).ReturnsAsync((users, 3));

            var service = CreateService(mockRepo);

            var result = await service.GetAllUsers(queryParams);

            Assert.Equal(3, result.Items.Count);
            Assert.Equal(3, result.TotalCount);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task AddUser_ReturnsCorrectDto()
        {
            var mockRepo = new Mock<IUserRepository>();

            var newUser = new AddUserDto { Username = "kevin", Password = "kevin123", Role = "student" };

            mockRepo.Setup(repo => repo.AddUser(It.IsAny<User>()))
                .ReturnsAsync((User u) =>
                {
                    u.id = 10;
                    return u;
                });

            var service = CreateService(mockRepo);

            var result = await service.AddUser(newUser);

            Assert.Equal(10, result.Id);
            Assert.Equal("kevin", result.Username);
            Assert.Equal("student", result.Role);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task AddUsers_ReturnsCorrectCount()
        {
            var mockRepo = new Mock<IUserRepository>();

            var newUsers = new List<AddUserDto>
            {
                new AddUserDto { Username = "kevin", Password = "kevin123", Role = "student" },
                new AddUserDto { Username = "grace", Password = "grace123", Role = "student" }
            };

            mockRepo.Setup(repo => repo.AddUsers(It.IsAny<List<User>>()))
                .ReturnsAsync((List<User> list) =>
                {
                    for (int i = 0; i < list.Count; i++) list[i].id = i + 1;
                    return list;
                });

            var service = CreateService(mockRepo);

            var result = await service.AddUsers(newUsers);

            Assert.Equal(2, result.Count);
            Assert.Equal("kevin", result[0].Username);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetUserById_ReturnsUser_WhenExists()
        {
            var mockRepo = new Mock<IUserRepository>();

            var user = new User { id = 1, Username = "kevin", Role = "student" };

            mockRepo.Setup(repo => repo.GetUserById(1)).ReturnsAsync(user);

            var service = CreateService(mockRepo);

            var result = await service.GetUserById(1);

            Assert.NotNull(result);
            Assert.Equal("kevin", result.Username);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetUserById_ReturnsNull_WhenNotFound()
        {
            var mockRepo = new Mock<IUserRepository>();

            mockRepo.Setup(repo => repo.GetUserById(99)).ReturnsAsync((User?)null);

            var service = CreateService(mockRepo);

            var result = await service.GetUserById(99);

            Assert.Null(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteUser_ReturnsTrue_WhenSuccessful()
        {
            var mockRepo = new Mock<IUserRepository>();

            mockRepo.Setup(repo => repo.DeleteUser(1)).ReturnsAsync(true);

            var service = CreateService(mockRepo);

            var result = await service.DeleteUser(1);

            Assert.True(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteUser_ReturnsFalse_WhenNotFound()
        {
            var mockRepo = new Mock<IUserRepository>();

            mockRepo.Setup(repo => repo.DeleteUser(99)).ReturnsAsync(false);

            var service = CreateService(mockRepo);

            var result = await service.DeleteUser(99);

            Assert.False(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateUser_ReturnsFalse_WhenUserDoesNotExist()
        {
            var mockRepo = new Mock<IUserRepository>();

            mockRepo.Setup(repo => repo.GetUserById(It.IsAny<int>())).ReturnsAsync((User?)null);

            var service = CreateService(mockRepo);

            var updateDto = new AddUserDto { Id = 99, Username = "ghost", Password = "", Role = "student" };

            var result = await service.UpdateUser(99, updateDto);

            Assert.False(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateUser_ReturnsTrue_WhenSuccessful()
        {
            var mockRepo = new Mock<IUserRepository>();

            var existingUser = new User { id = 1, Username = "kevin", Role = "student", PasswordHarsh = "oldhash" };

            mockRepo.Setup(repo => repo.GetUserById(1)).ReturnsAsync(existingUser);
            mockRepo.Setup(repo => repo.UpdateUser(It.IsAny<User>())).ReturnsAsync(true);

            var service = CreateService(mockRepo);

            var updateDto = new AddUserDto { Id = 1, Username = "kevin updated", Password = "", Role = "student" };

            var result = await service.UpdateUser(1, updateDto);

            Assert.True(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task UserLogin_ReturnsNull_WhenUserDoesNotExist()
        {
            var mockRepo = new Mock<IUserRepository>();

            mockRepo.Setup(repo => repo.UserExists("ghost")).ReturnsAsync((User?)null);

            var service = CreateService(mockRepo);

            var loginDto = new LoginUserDto { Username = "ghost", Password = "whatever" };

            var result = await service.UserLogin(loginDto);

            Assert.Null(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task UserLogin_ReturnsNull_WhenPasswordIsWrong()
        {
            var mockRepo = new Mock<IUserRepository>();
            var hasher = new PasswordHasher<User>();

            var user = new User { id = 1, Username = "kevin", Role = "student" };
            user.PasswordHarsh = hasher.HashPassword(user, "correctpassword");

            mockRepo.Setup(repo => repo.UserExists("kevin")).ReturnsAsync(user);

            var service = CreateService(mockRepo);

            var loginDto = new LoginUserDto { Username = "kevin", Password = "wrongpassword" };

            var result = await service.UserLogin(loginDto);

            Assert.Null(result);
        }
    }
}