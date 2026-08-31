using System.Net;
using System.Net.Http.Json;
using WA1.DTOs;
using Xunit;

namespace WA1.Tests
{
    public class CourseControllerTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public CourseControllerTests(CustomWebApplicationFactory factory)
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
        public async Task GetAllCourses_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/courses");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task AddCourse_ThenGetCourseById_ReturnsCourse()
        {
            var newCourse = new AddCourseDto { Name = "Integration Course", Duration = 3, Lecturer = "Dr. Test", Fees = 750 };

            var createResponse = await _client.PostAsJsonAsync("/api/courses", newCourse);
            createResponse.EnsureSuccessStatusCode();

            var created = await createResponse.Content.ReadFromJsonAsync<CourseDto>();
            Assert.True(created.Id > 0);

            var getResponse = await _client.GetAsync($"/api/courses/{created.Id}");
            getResponse.EnsureSuccessStatusCode();

            var fetched = await getResponse.Content.ReadFromJsonAsync<CourseDto>();
            Assert.Equal("Integration Course", fetched.Name);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task UpdateCourse_ThenDelete_WorksCorrectly()
        {
            var newCourse = new AddCourseDto { Name = "Update Course", Duration = 2, Lecturer = "Dr. Original", Fees = 400 };
            var createResponse = await _client.PostAsJsonAsync("/api/courses", newCourse);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<CourseDto>();

            var updateDto = new AddCourseDto { Name = "Update Course", Duration = 2, Lecturer = "Dr. Updated", Fees = 450 };
            var updateResponse = await _client.PutAsJsonAsync($"/api/courses/{created.Id}", updateDto);
            Assert.True(updateResponse.IsSuccessStatusCode, $"Update failed: {updateResponse.StatusCode}");

            var getResponse = await _client.GetAsync($"/api/courses/{created.Id}");
            var fetched = await getResponse.Content.ReadFromJsonAsync<CourseDto>();
            Assert.Equal("Dr. Updated", fetched.Lecturer);

            var deleteResponse = await _client.DeleteAsync($"/api/courses/{created.Id}");
            Assert.True(deleteResponse.IsSuccessStatusCode, $"Delete failed: {deleteResponse.StatusCode}");

            var getAfterDelete = await _client.GetAsync($"/api/courses/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getAfterDelete.StatusCode);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetCourseById_ReturnsNotFound_WhenCourseDoesNotExist()
        {
            var response = await _client.GetAsync("/api/courses/9999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}