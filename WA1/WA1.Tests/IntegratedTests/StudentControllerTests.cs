using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WA1.DTOs;
using Xunit;
using Xunit.Abstractions;

namespace WA1.Tests
{
    public class StudentControllerTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public StudentControllerTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _output = output;
        }

        public async Task InitializeAsync()
        {
            await _factory.InitializeDatabaseAsync();
            await AuthenticateAsAdminAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private async Task AuthenticateAsAdminAsync()
        {
            var loginDto = new LoginUserDto { Username = "testadmin", Password = "AdminPass123!" };
            var loginResponse = await _client.PostAsJsonAsync("/api/users/login", loginDto);
            loginResponse.EnsureSuccessStatusCode();

            var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
            _output.WriteLine($"Logged in as: {auth.User.Username}, Role: {auth.User.Role}");
            _output.WriteLine($"Token: {auth.Token}");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", auth.Token);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetAllStudents_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/students");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task AddCourse_ThenAddStudent_LinksCorrectly()
        {
            var newCourse = new AddCourseDto { Name = "Test Course", Duration = 2, Lecturer = "Dr. Test", Fees = 500 };
            var courseResponse = await _client.PostAsJsonAsync("/api/courses", newCourse);

            var courseBody = await courseResponse.Content.ReadAsStringAsync();
            Assert.True(courseResponse.IsSuccessStatusCode, $"Course creation failed: {courseResponse.StatusCode} — {courseBody}");

            var course = await courseResponse.Content.ReadFromJsonAsync<CourseDto>();

            var newStudent = new AddStudentDto
            {
                Name = "Test Student",
                Age = 20,
                Email = "teststudent@email.com",
                CourseId = course.Id
            };

            var studentResponse = await _client.PostAsJsonAsync("/api/students/add", newStudent);

            var studentBody = await studentResponse.Content.ReadAsStringAsync();
            Assert.True(studentResponse.IsSuccessStatusCode, $"Student creation failed: {studentResponse.StatusCode} — {studentBody}");

            var student = await studentResponse.Content.ReadFromJsonAsync<StudentDto>();
            Assert.Equal(course.Id, student.CourseId);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task UpdateStudent_ThenDelete_WorksCorrectly()
        {
            // seed a course + student first
            var newCourse = new AddCourseDto { Name = "Update Test Course", Duration = 2, Lecturer = "Dr. Test", Fees = 500 };
            var courseResponse = await _client.PostAsJsonAsync("/api/courses", newCourse);
            courseResponse.EnsureSuccessStatusCode();
            var course = await courseResponse.Content.ReadFromJsonAsync<CourseDto>();

            var newStudent = new AddStudentDto
            {
                Name = "Update Test Student",
                Age = 20,
                Email = "updatetest@email.com",
                CourseId = course.Id
            };
            var createResponse = await _client.PostAsJsonAsync("/api/students/add", newStudent);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<StudentDto>();

            // update
            var updateDto = new UpdateStudentDto
            {
                Name = "Updated Name",
                Age = 21,
                Email = "updatetest@email.com",
                CourseId = course.Id
            };
            var updateResponse = await _client.PutAsJsonAsync($"/api/students/{created.Id}", updateDto);
            Assert.True(updateResponse.IsSuccessStatusCode, $"Update failed: {updateResponse.StatusCode}");

            // confirm the update actually took
            var getResponse = await _client.GetAsync($"/api/students/{created.Id}");
            getResponse.EnsureSuccessStatusCode();
            var fetched = await getResponse.Content.ReadFromJsonAsync<StudentDto>();
            Assert.Equal("Updated Name", fetched.Name);

            // delete
            var deleteResponse = await _client.DeleteAsync($"/api/students/{created.Id}");
            Assert.True(deleteResponse.IsSuccessStatusCode, $"Delete failed: {deleteResponse.StatusCode}");

            // confirm it's actually gone
            var getAfterDelete = await _client.GetAsync($"/api/students/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getAfterDelete.StatusCode);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task AddStudent_ReturnsForbidden_WhenUserIsNotAdmin()
        {
            // register + log in as a plain, non-admin user
            var registerDto = new AddUserDto { Username = "plainuser", Password = "PlainPass123!", Role = "Student" };
            var registerResponse = await _client.PostAsJsonAsync("/api/users/register", registerDto);
            registerResponse.EnsureSuccessStatusCode();

            var loginDto = new LoginUserDto { Username = "plainuser", Password = "PlainPass123!" };
            var loginResponse = await _client.PostAsJsonAsync("/api/users/login", loginDto);
            loginResponse.EnsureSuccessStatusCode();
            var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

            // swap the client's token from admin to this plain user for this one test
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", auth.Token);

            var newStudent = new AddStudentDto
            {
                Name = "Should Not Be Created",
                Age = 20,
                Email = "shouldfail@email.com",
                CourseId = 1
            };

            var response = await _client.PostAsJsonAsync("/api/students/add", newStudent);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}