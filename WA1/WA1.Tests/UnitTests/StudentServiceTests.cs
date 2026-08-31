using Moq;
using WA1.DTOs;
using WA1.Models;
using WA1.Repositories;
using WA1.Services;
using Microsoft.Extensions.Logging;
using Xunit;

namespace WA1.Tests
{
	public class StudentServiceTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetStudentByIdAsync_ReturnsCorrectStudent_WhenStudentExists()
        {
            // Arrange — set up the fake repository and its canned response
            var mockRepo = new Mock<IStudentRepository>();

            var fakeStudent = new Student
            {
                Id = 1,
                Name = "Kevin",
                Age = 20,
                Email = "kevin@email.com",
                CourseId = 1,
                Course = new Course { Id = 1, Name = "Computer Science", Lecturer = "Dr. Johnson" }
            };

            mockRepo.Setup(repo => repo.GetStudentById(1)).ReturnsAsync(fakeStudent);

            var mockLogger = new Mock<ILogger<StudentService>>();
            var service = new StudentService(mockRepo.Object, mockLogger.Object);

            // Act — call the actual method being tested
            var result = await service.GetStudentById(1);

            // Assert — check the outcome is what we expect
            Assert.NotNull(result);
            Assert.Equal("Kevin", result.Name);
            Assert.Equal("Computer Science", result.CourseName);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetStudentById_ReturnsNull_WhenStudentDoesNotExist()
        {
            var mockRepo = new Mock<IStudentRepository>();
            mockRepo.Setup(repo => repo.GetStudentById(99)).ReturnsAsync((Student?)null);

            var mockLogger = new Mock<ILogger<StudentService>>();
            var service = new StudentService(mockRepo.Object, mockLogger.Object);

            var result = await service.GetStudentById(99);

            Assert.Null(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task AddStudent_ReturnsCorrectDto_WhenSuccessful()
        {
            var mockRepo = new Mock<IStudentRepository>();
            var mockLogger = new Mock<ILogger<StudentService>>();

            var addDto = new AddStudentDto
            {
                Name = "Grace",
                Age = 19,
                Email = "grace@email.com",
                CourseId = 2,
                UserId = 5
            };

            // whatever the repo returns simulates what the "saved" entity would look like,
            // including an auto-generated Id
            mockRepo.Setup(repo => repo.AddStudent(It.IsAny<Student>()))
                .ReturnsAsync((Student s) =>
                {
                    s.Id = 10; // simulate the database assigning an id
                    s.Course = new Course { Id = 2, Name = "Computer Science", Lecturer = "Dr. Johnson" };
                    return s;
                });

            var service = new StudentService(mockRepo.Object, mockLogger.Object);

            var result = await service.AddStudent(addDto);

            Assert.Equal(10, result.Id);
            Assert.Equal("Grace", result.Name);
            Assert.Equal("Computer Science", result.CourseName);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateStudentInfo_ReturnsTrue_WhenStudentExists()
        {
            var mockRepo = new Mock<IStudentRepository>();
            var mockLogger = new Mock<ILogger<StudentService>>();

            mockRepo.Setup(repo => repo.UpdateStudentInfo(It.IsAny<Student>())).ReturnsAsync(true);

            var service = new StudentService(mockRepo.Object, mockLogger.Object);

            var updateDto = new UpdateStudentDto { Name = "Kevin Updated", Age = 21, Email = "kevin@email.com", CourseId = 1 };

            var result = await service.UpdateStudentInfo(1, updateDto);

            Assert.True(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateStudentInfo_ReturnsFalse_WhenStudentDoesNotExist()
        {
            var mockRepo = new Mock<IStudentRepository>();
            var mockLogger = new Mock<ILogger<StudentService>>();

            mockRepo.Setup(repo => repo.UpdateStudentInfo(It.IsAny<Student>())).ReturnsAsync(false);

            var service = new StudentService(mockRepo.Object, mockLogger.Object);

            var updateDto = new UpdateStudentDto { Name = "Ghost", Age = 0, Email = "x@x.com", CourseId = 1 };

            var result = await service.UpdateStudentInfo(999, updateDto);

            Assert.False(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteStudent_ReturnsTrue_WhenStudentExists()
        {
            var mockRepo = new Mock<IStudentRepository>();
            var mockLogger = new Mock<ILogger<StudentService>>();

            mockRepo.Setup(repo => repo.DeleteStudent(1)).ReturnsAsync(true);

            var service = new StudentService(mockRepo.Object, mockLogger.Object);

            var result = await service.DeleteStudent(1);

            Assert.True(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteStudent_ReturnsFalse_WhenStudentDoesNotExist()
        {
            var mockRepo = new Mock<IStudentRepository>();
            var mockLogger = new Mock<ILogger<StudentService>>();

            mockRepo.Setup(repo => repo.DeleteStudent(99)).ReturnsAsync(false);

            var service = new StudentService(mockRepo.Object, mockLogger.Object);

            var result = await service.DeleteStudent(99);

            Assert.False(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetStudentByCourseId_ReturnsMatchingStudents()
        {
            var mockRepo = new Mock<IStudentRepository>();
            var mockLogger = new Mock<ILogger<StudentService>>();

            var students = new List<Student>
            {
                new Student { Id = 1, Name = "Kevin", CourseId = 2, Course = new Course { Id = 2, Name = "Computer Science" } },
                new Student { Id = 2, Name = "Grace", CourseId = 2, Course = new Course { Id = 2, Name = "Computer Science" } }
            };

            mockRepo.Setup(repo => repo.GetStudentByCourseId(2)).ReturnsAsync(students);

            var service = new StudentService(mockRepo.Object, mockLogger.Object);

            var result = await service.GetStudentByCourseId(2);

            Assert.Equal(2, result.Count);
            Assert.All(result, s => Assert.Equal("Computer Science", s.CourseName));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task AddStudents_ReturnsCorrectCount()
        {
            var mockRepo = new Mock<IStudentRepository>();
            var mockLogger = new Mock<ILogger<StudentService>>();

            var addDtos = new List<AddStudentDto>
            {
                new AddStudentDto { Name = "Kevin", CourseId = 1, UserId = 1 },
                new AddStudentDto { Name = "Grace", CourseId = 1, UserId = 2 }
            };

            mockRepo.Setup(repo => repo.AddStudents(It.IsAny<List<Student>>()))
                .ReturnsAsync((List<Student> list) =>
                {
                    for (int i = 0; i < list.Count; i++) list[i].Id = i + 1;
                    return list;
                });

            var service = new StudentService(mockRepo.Object, mockLogger.Object);

            var result = await service.AddStudents(addDtos);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllStudents_ReturnsCorrectPagedResult()
        {
            var mockRepo = new Mock<IStudentRepository>();
            var mockLogger = new Mock<ILogger<StudentService>>();

            var students = new List<Student>
            {
                new Student { Id = 1, Name = "Kevin", Course = new Course { Name = "CS" } },
                new Student { Id = 2, Name = "Grace", Course = new Course { Name = "CS" } }
            };

            var queryParams = new StudentQueryParams { Page = 1, PageSize = 2 };

            mockRepo.Setup(repo => repo.GetFiltered(queryParams)).ReturnsAsync((students, 5)); // pretend 5 total match, only 2 on this page

            var service = new StudentService(mockRepo.Object, mockLogger.Object);

            var result = await service.GetAllStudents(queryParams);

            Assert.Equal(2, result.Items.Count);
            Assert.Equal(5, result.TotalCount);
            Assert.Equal(1, result.Page);
        }
    }
}
