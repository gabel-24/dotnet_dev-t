using WA1.Models;
using WA1.DTOs;

namespace WA1.Services
{
    public interface ICourseService
    {
        Task<PagedResult<CourseDto>> GetAllCourse(CourseQueryParams queryParams);
        Task<CourseDto?> GetCourseById(int id);
        Task<bool> UpdateCourseInfo(int id, AddCourseDto updatedCourse);
        Task<CourseDto> AddCourse(AddCourseDto newCourse);
        Task<bool> DeleteCourse(int id);
    }
}
