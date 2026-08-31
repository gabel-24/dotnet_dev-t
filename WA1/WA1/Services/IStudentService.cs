using WA1.DTOs;
using WA1.Models;

namespace WA1.Services
{
    public interface IStudentService
    {
        Task<PagedResult<StudentDto>> GetAllStudents(StudentQueryParams queryParams);
        Task<StudentDto?> GetStudentById(int id);
        Task<StudentDto> AddStudent(AddStudentDto addDto);
        Task<bool> UpdateStudentInfo(int id, UpdateStudentDto updateDto);
        Task<bool> DeleteStudent(int id);
        Task<List<StudentDto>> GetStudentByCourseId(int courseId);
        Task<List<StudentDto>> AddStudents(List<AddStudentDto> newStudents);
    }
}
