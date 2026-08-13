using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WA1.Data;
using WA1.DTOs;
using WA1.Models;

namespace WA1.Repositories
{
    public class StudentRepository: IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Student>> GetAllStudents() 
        {
            return await _context.Students
                .Include(s => s.Course)
                .ToListAsync();
        }
        public async Task<Student?> GetStudentById(int id) 
        {
            return await _context.Students
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<Student> AddStudent(Student addStudent) 
        {
            _context.Students.Add(addStudent);
            await _context.SaveChangesAsync();

            await _context.Entry(addStudent).Reference(s => s.Course).LoadAsync();
            return addStudent;
        }
        public async Task<bool> UpdateStudentInfo(Student updatedStudent) 
        {
            var student = _context.Students.Find(updatedStudent.Id);

            if(student == null)
            {
                return false;
            }

            student.Name = updatedStudent.Name;
            student.Age = updatedStudent.Age;
            student.Email = updatedStudent.Email;
            student.CourseId = updatedStudent.CourseId;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if(student == null)
            {
                return false;
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync() ;
            return true;
        }
        public async Task<List<Student>> GetStudentByCourseId(int courseId) 
        {
            return await _context.Students
                .Include(s => s.CourseId)
                .Where(s => s.CourseId == courseId)
                .ToListAsync();
        }
        public async Task<List<Student>> AddStudents(List<Student> newStudents) 
        {
            _context.Students.AddRange(newStudents);
            await _context.SaveChangesAsync();

            foreach(var student in newStudents)
            {
                await _context.Entry(student).Reference(x => x.Course).LoadAsync();
            }

            return newStudents;
        }
        public async Task<(List<Student> Students, int TotalCount)> GetFiltered(StudentQueryParams queryParams)
        {
            var query = _context.Students
                                    .Include(s => s.Course)
                                    .AsQueryable(); // start building a composable query

            // filtering
            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                var search = queryParams.Search.ToLower();
                query = query.Where(s => s.Name.ToLower().Contains(search) || s.Email.ToLower().Contains(search));
            }

            if (queryParams.CourseId.HasValue)
            {
                query = query.Where(s => s.CourseId == queryParams.CourseId.Value);
            }

            // sorting
            query = queryParams.SortBy.ToLower() switch
            {
                "age" => queryParams.Descending ? query.OrderByDescending(s => s.Age) : query.OrderBy(s => s.Age),
                "email" => queryParams.Descending ? query.OrderByDescending(s => s.Email) : query.OrderBy(s => s.Email),
                _ => queryParams.Descending ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name)
            };

            // count BEFORE paging — this is the total across all pages, not just this page
            int totalCount = await query.CountAsync();

            // pagination
            var students = await query
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return (students, totalCount);

        }
    }
}
