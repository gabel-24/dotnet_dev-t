using WA1.DTOs;
using WA1.Models;

namespace WA1.Repositories
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAllStudents();
        Task<Student?> GetStudentById(int id);
        Task<Student> AddStudent(Student addStudent);
        Task<bool> UpdateStudentInfo(Student updatedStudent);
        Task<bool> DeleteStudent(int id);
        Task<List<Student>> GetStudentByCourseId(int courseId);
        Task<List<Student>> AddStudents(List<Student> newStudents);
        Task<(List<Student> Students, int TotalCount)> GetFiltered(StudentQueryParams queryParams);
    }
}
