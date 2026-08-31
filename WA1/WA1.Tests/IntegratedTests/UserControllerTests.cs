using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WA1.DTOs;
using Xunit;

namespace WA1.Tests
{
    public class UserControllerTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public UserControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        public async Task InitializeAsync()
        {
            await _factory.InitializeDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Register_ThenLogin_ReturnsValidToken()
        {
            var registerDto = new AddUserDto { Username = "integrationuser", Password = "IntegrationPass123!", Role = "Student" };
            var registerResponse = await _client.PostAsJsonAsync("/api/users/register", registerDto);
            registerResponse.EnsureSuccessStatusCode();

            var loginDto = new LoginUserDto { Username = "integrationuser", Password = "IntegrationPass123!" };
            var loginResponse = await _client.PostAsJsonAsync("/api/users/login", loginDto);
            loginResponse.EnsureSuccessStatusCode();

            var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
            Assert.False(string.IsNullOrWhiteSpace(auth.Token));
            Assert.Equal("integrationuser", auth.User.Username);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Login_ReturnsUnauthorized_WithWrongPassword()
        {
            var registerDto = new AddUserDto { Username = "wrongpassuser", Password = "CorrectPass123!", Role = "Student" };
            await _client.PostAsJsonAsync("/api/users/register", registerDto);

            var loginDto = new LoginUserDto { Username = "wrongpassuser", Password = "WrongPass123!" };
            var loginResponse = await _client.PostAsJsonAsync("/api/users/login", loginDto);

            Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetUserById_ReturnsUnauthorized_WithoutToken()
        {
            var response = await _client.GetAsync("/api/users/1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetUserById_ReturnsOk_WithValidToken()
        {
            var registerDto = new AddUserDto { Username = "authcheckuser", Password = "AuthCheck123!", Role = "Student" };
            var registerResponse = await _client.PostAsJsonAsync("/api/users/register", registerDto);
            var created = await registerResponse.Content.ReadFromJsonAsync<UserDto>();

            var loginDto = new LoginUserDto { Username = "authcheckuser", Password = "AuthCheck123!" };
            var loginResponse = await _client.PostAsJsonAsync("/api/users/login", loginDto);
            var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);

            var response = await _client.GetAsync($"/api/users/{created.Id}");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task AddUsersBulk_ReturnsForbidden_WhenUserIsNotAdmin()
        {
            var registerDto = new AddUserDto { Username = "notadmin", Password = "NotAdmin123!", Role = "Student" };
            await _client.PostAsJsonAsync("/api/users/register", registerDto);

            var loginDto = new LoginUserDto { Username = "notadmin", Password = "NotAdmin123!" };
            var loginResponse = await _client.PostAsJsonAsync("/api/users/login", loginDto);
            var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);

            var bulkUsers = new List<AddUserDto>
            {
                new AddUserDto { Username = "bulk1", Password = "Bulk123!", Role = "Student" }
            };

            var response = await _client.PostAsJsonAsync("/api/users/register/bulk", bulkUsers);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}