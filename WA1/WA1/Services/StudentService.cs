using Microsoft.EntityFrameworkCore;
using WA1.Data;
using WA1.DTOs;
using WA1.Models;
using WA1.Repositories;

namespace WA1.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;
        private readonly ILogger<StudentService> _logger;

        public StudentService(IStudentRepository repository, ILogger<StudentService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        private static StudentDto ToDto(Student s) => new StudentDto
        {
            Id = s.Id,
            Name = s.Name,
            Age = s.Age,
            Email = s.Email,
            CourseId = s.CourseId,
            CourseName = s.Course?.Name,
            Lecturer = s.Course?.Lecturer
        };

        public async Task<PagedResult<StudentDto>> GetAllStudents(StudentQueryParams queryParams)
        {
            var (students, totalCount) = await _repository.GetFiltered(queryParams);

            return new PagedResult<StudentDto>
            {
                Items = students.Select(ToDto).ToList(),
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };

        }

        public async Task<StudentDto?> GetStudentById(int id)
        {
            var student = await _repository.GetStudentById(id);
            
            if(student == null)
            {
                _logger.LogWarning("Attempting to get student of id {id}, but no such student exists.", id);
                return null;
            }
            else
            {
                return ToDto(student);
            }
        }

        public async Task<StudentDto> AddStudent(AddStudentDto newStudent)
        {
            _logger.LogInformation("Creating student {name} for course {courseId}", newStudent.Name, newStudent.CourseId);
            var student = new Student
            {
                Name = newStudent.Name,
                Age = newStudent.Age,
                Email = newStudent.Email,
                CourseId = newStudent.CourseId,
                UserId = newStudent.UserId
            };

            var created = await _repository.AddStudent(student);
            _logger.LogInformation("Student created with id {id}", created.Id);
            return ToDto(created);
        }

        public async Task<bool> UpdateStudentInfo(int id, UpdateStudentDto updatedStudent)
        {
            _logger.LogInformation("Updating information of {name} student", updatedStudent.Name);

            var student = new Student
            {
                Id = id,
                Name = updatedStudent.Name,
                Age = updatedStudent.Age,
                Email = updatedStudent.Email,
                CourseId = updatedStudent.CourseId,
            };

            _logger.LogInformation("Updated student {name}'s info", updatedStudent.Name);
            return await _repository.UpdateStudentInfo(student);
        }

        public async Task<bool> DeleteStudent(int id)
        {
            var success = await _repository.DeleteStudent(id);

            if (!success)
            {
                _logger.LogWarning("Attempting to delete information of student with id {id} but no such student with id exists", id);
            }
            else
            {
                _logger.LogInformation("Deleted student info with id {id}", id);
            }
            return success;
        }

        public async Task<List<StudentDto>> GetStudentByCourseId(int courseId)
        {
            var students = await _repository.GetStudentByCourseId(courseId);
            return students.Select(ToDto).ToList();
        }

        public async Task<List<StudentDto>> AddStudents(List<AddStudentDto> newStudents)
        {
            _logger.LogInformation("Adding students ");
            var students = newStudents.Select(s => new Student
            {
                Name = s.Name,
                Age = s.Age,
                Email = s.Email,
                CourseId = s.CourseId,
                UserId = s.UserId,
            }).ToList();

            _logger.LogInformation("Added students");
            var created = await _repository.AddStudents(students);

            return created.Select(ToDto).ToList();
        }
    }
}