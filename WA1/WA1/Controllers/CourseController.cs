using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using WA1.Models;
using WA1.Services;
using WA1.DTOs;

namespace WA1.Controllers
{
    [ApiController]
    [Route("api/courses")]

    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCourses([FromQuery] CourseQueryParams queryParams)
        {
            var courses = await _courseService.GetAllCourse(queryParams);
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await _courseService.GetCourseById(id);

            if(course == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(course);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourseInfo(int id, AddCourseDto updatedCourse)
        {
            bool success = await _courseService.UpdateCourseInfo(id, updatedCourse);

            if(!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> AddCourse(AddCourseDto newCourse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = await _courseService.AddCourse(newCourse);

            return CreatedAtAction(nameof(GetCourseById), new { id = created.Id }, created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            bool success = await _courseService.DeleteCourse(id);

            if(!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
