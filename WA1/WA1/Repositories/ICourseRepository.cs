using WA1.DTOs;
using WA1.Models;

namespace WA1.Repositories
{
    public interface ICourseRepository
    {
        Task<List<Course>> GetAllCourse();
        Task<Course?> GetCourseById(int id);
        Task<bool> UpdateCourseInfo(Course updatedCourse);
        Task<Course> AddCourse(Course newCourse);
        Task<bool> DeleteCourse(int id);
        Task<(List<Course> Courses, int TotalCount)> GetFiltered(CourseQueryParams queryParams);
    }
}
