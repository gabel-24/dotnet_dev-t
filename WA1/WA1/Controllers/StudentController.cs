using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WA1.DTOs;
using WA1.Services;

namespace WA1.Controllers
{
    [ApiController]
    [Route("api/students")] // lowercase convention, but works either way since routing is case-insensitive
    public class StudentController : ControllerBase // was: Controller — ControllerBase is correct for a pure API, no view support needed
    {
        private readonly IStudentService _studentService;
        private readonly IUserService _userService;

        public StudentController(IStudentService studentService, IUserService userService)
        {
            _studentService = studentService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStudents([FromQuery] StudentQueryParams queryParams)
        {
            var students = await _studentService.GetAllStudents(queryParams);
            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var student = await _studentService.GetStudentById(id);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddStudent(AddStudentDto newStudent) // was: Student — now takes the DTO
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. create the login account
            var newUser = new AddUserDto
            {
                Username = newStudent.Name,
                Password = "Welcome123!",
                Role = "Student"
            };

            var createdUser = await _userService.AddUser(newUser);

            newStudent.UserId = createdUser.Id;

            var created = await _studentService.AddStudent(newStudent);

            return CreatedAtAction(nameof(GetStudentById), new { id = created.Id }, created);
        }

        [HttpPost("addmany")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddStudents(List<AddStudentDto> students) // was: List<Student>
        {
            foreach (var student in students)
            {
                var newUser = new AddUserDto
                {
                    Username = student.Name,
                    Password = "Welcome123!",
                    Role = "Student"
                };

                var createdUser = await _userService.AddUser(newUser);
                student.UserId = createdUser.Id; // attach the new user's id onto the same DTO
            }

            var addedStudents = await _studentService.AddStudents(students);

            return Ok(addedStudents); // was: Ok(students) — returning the input instead of the actual saved result
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Lecturer")]
        public async Task<IActionResult> UpdateStudentInfo(int id, UpdateStudentDto updatedStudent) // was: Student
        {
            bool success = await _studentService.UpdateStudentInfo(id, updatedStudent);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            bool success = await _studentService.DeleteStudent(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("course/{courseId}")]
        [Authorize(Roles = "Admin,Lecturer")]
        public async Task<IActionResult> GetStudentByCourseId(int courseId)
        {
            var matchingStudents = await _studentService.GetStudentByCourseId(courseId);

            if (matchingStudents.Count == 0)
            {
                return NotFound();
            }

            return Ok(matchingStudents);
        }
    }
}