using Microsoft.EntityFrameworkCore;
using WA1.Data;
using WA1.Models;
using WA1.DTOs;
using WA1.Repositories;
using System.Diagnostics;

namespace WA1.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repository;

        public CourseService(ICourseRepository repository)
        {
            _repository = repository;
        }

        private static CourseDto ToDto(Course c) => new CourseDto
        {
            Id = c.Id,
            Name = c.Name,
            Duration = c.Duration,
            Lecturer = c.Lecturer,
            Fees = c.Fees,
            StudentCount = c.Students.Count,
        };

        public async Task<PagedResult<CourseDto>> GetAllCourse(CourseQueryParams queryParams)
        {
            var (courses, totalCount) = await _repository.GetFiltered(queryParams);

            return new PagedResult<CourseDto>
            {
                Items = courses.Select(ToDto).ToList(),
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<CourseDto?> GetCourseById(int id)
        {
            var course = await _repository.GetCourseById(id);
            return course == null ? null : ToDto(course);
        }

        public async Task<bool> UpdateCourseInfo(int id, AddCourseDto updatedCourse)
        {
            var course = new Course
            {
                Name = updatedCourse.Name,
                Duration = updatedCourse.Duration,
                Lecturer = updatedCourse.Lecturer,
                Fees = updatedCourse.Fees
            };

            return await _repository.UpdateCourseInfo(course);

        }

        public async Task<CourseDto> AddCourse(AddCourseDto newCourse)
        {
            var course = new Course
            {
                Name = newCourse.Name,
                Duration = newCourse.Duration,
                Lecturer = newCourse.Lecturer,
                Fees = newCourse.Fees
            };

            var created = await _repository.AddCourse(course);
            return ToDto(course);
           
        }

        public async Task<bool> DeleteCourse(int id)
        {
           return await _repository.DeleteCourse(id);
        }

    }
}
