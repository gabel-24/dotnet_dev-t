using System.ComponentModel.DataAnnotations;

namespace WA1.DTOs
{
    public class AddCourseDto
    {
        [Required]
        public string? Name {  get; set; }
        [Range(1,5)]
        public int Duration { get; set; }
        [Required]
        public string? Lecturer { get; set; }
        [Required]
        public int Fees { get; set; }
    }
}
