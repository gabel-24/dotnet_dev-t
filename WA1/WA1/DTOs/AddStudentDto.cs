using System.ComponentModel.DataAnnotations;

namespace WA1.DTOs
{
    public class AddStudentDto
    {
        [Required]
        public string? Name {  get; set; }
        [Range(16,100)]
        public int Age {  get; set; }
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public int CourseId { get; set; }
        public int UserId {  get; set; }
    }
}
