using Microsoft.EntityFrameworkCore;
using WA1.Data;
using WA1.DTOs;
using WA1.Models;

namespace WA1.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private AppDbContext _context;

        public CourseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Course>> GetAllCourse()
        {
            return await _context.Courses.ToListAsync();
        }
        public async Task<Course?> GetCourseById(int id)
        {
            return await _context.Courses.FindAsync(id);
        }
        public async Task<bool> UpdateCourseInfo(Course updatedCourse)
        {
            var course = await _context.Courses.FindAsync(updatedCourse.Id);

            if(course == null)
            {
                return false;
            }

            course.Lecturer = updatedCourse.Lecturer;
            course.Duration = updatedCourse.Duration;
            course.Fees = updatedCourse.Fees;
            course.Name = updatedCourse.Name;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Course> AddCourse(Course newCourse)
        {
            _context.Courses.Add(newCourse);
            await _context.SaveChangesAsync();

            return newCourse;
        }
        public async Task<bool> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if(course == null)
            {
                return false;
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync() ;
            return true;

        }
        public async Task<(List<Course> Courses, int TotalCount)> GetFiltered(CourseQueryParams queryParams)
        {
            var query = _context.Courses
                            .AsQueryable();

            //filter
            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                var search = queryParams.Search;
                query = query.Where(c => c.Name.ToLower().Contains(search));
            }

            //sorting
            query = queryParams.SortBy.ToLower() switch
            {
                "fees" => queryParams.Descending ? query.OrderByDescending(c => c.Fees) : query.OrderBy(c => c.Fees),
                "duration" => queryParams.Descending ? query.OrderByDescending(c => c.Duration) : query.OrderBy(c => c.Duration),
                _ => queryParams.Descending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name)
            };

            //count results b4 paging
            int totalCount = await query.CountAsync();

            //pagination
            var courses = await query
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return (courses, totalCount);
        }
    }
}
