using Moq;
using WA1.DTOs;
using WA1.Models;
using WA1.Repositories;
using WA1.Services;
using Xunit;

namespace WA1.Tests
{
    public class CourseServiceTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllCourse_ReturnsCorrectPagedResult()
        {
            var mockRepo = new Mock<ICourseRepository>();

            var courses = new List<Course>
            {
                new Course { Id = 1, Name = "Cs", Duration = 3, Lecturer = "Ks", Fees = 100 },
                new Course { Id = 2, Name = "BaF", Duration = 3, Lecturer = "SW", Fees = 150 }
            };

            var queryParams = new CourseQueryParams();

            mockRepo.Setup(repo => repo.GetFiltered(queryParams)).ReturnsAsync((courses, 2));

            var service = new CourseService(mockRepo.Object);

            var result = await service.GetAllCourse(queryParams);

            Assert.Equal(2, result.Items.Count);
            Assert.Equal(2, result.TotalCount);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCourseById_ReturnsCourse_WhenExists()
        {
            var mockRepo = new Mock<ICourseRepository>();

            var course = new Course { Id = 1, Name = "Cs", Duration = 3, Lecturer = "Ks", Fees = 100 };

            mockRepo.Setup(repo => repo.GetCourseById(1)).ReturnsAsync(course);

            var service = new CourseService(mockRepo.Object);

            var result = await service.GetCourseById(1);

            Assert.NotNull(result);
            Assert.Equal("Cs", result.Name);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCourseById_ReturnsNull_WhenNotFound()
        {
            var mockRepo = new Mock<ICourseRepository>();

            mockRepo.Setup(repo => repo.GetCourseById(99)).ReturnsAsync((Course?)null);

            var service = new CourseService(mockRepo.Object);

            var result = await service.GetCourseById(99);

            Assert.Null(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task AddCourse_ReturnsCorrectDto()
        {
            var mockRepo = new Mock<ICourseRepository>();

            var newCourse = new AddCourseDto { Name = "Cs", Duration = 3, Lecturer = "Ks", Fees = 100 };

            mockRepo.Setup(repo => repo.AddCourse(It.IsAny<Course>()))
                .ReturnsAsync((Course c) =>
                {
                    c.Id = 5;
                    return c;
                });

            var service = new CourseService(mockRepo.Object);

            var result = await service.AddCourse(newCourse);

            Assert.Equal(5, result.Id);
            Assert.Equal("Cs", result.Name);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateCourseInfo_ReturnsTrue_WhenSuccessful()
        {
            var mockRepo = new Mock<ICourseRepository>();

            mockRepo.Setup(repo => repo.UpdateCourseInfo(It.IsAny<Course>())).ReturnsAsync(true);

            var service = new CourseService(mockRepo.Object);

            var updateDto = new AddCourseDto { Name = "Cs Updated", Duration = 4, Lecturer = "Ks", Fees = 120 };

            var result = await service.UpdateCourseInfo(1, updateDto);

            Assert.True(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCourse_ReturnsFalse_WhenNotFound()
        {
            var mockRepo = new Mock<ICourseRepository>();

            mockRepo.Setup(repo => repo.DeleteCourse(99)).ReturnsAsync(false);

            var service = new CourseService(mockRepo.Object);

            var result = await service.DeleteCourse(99);

            Assert.False(result);
        }
    }
}