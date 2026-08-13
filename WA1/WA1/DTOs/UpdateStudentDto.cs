using System.ComponentModel.DataAnnotations;

namespace WA1.DTOs
{
    public class UpdateStudentDto
    {
        [Required]
        public string? Name {  get; set; }
        [Range(16,100)]
        public int Age {  get; set; }
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public int CourseId { get; set; }
    }
}
