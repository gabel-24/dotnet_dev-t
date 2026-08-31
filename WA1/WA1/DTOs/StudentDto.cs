namespace WA1.DTOs
{
    public class StudentDto// for returning student data
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
        public string? Email { get; set; }
        public int CourseId { get; set; }
        public string? CourseName { get; set; }  // Instead of full Course object
        public string? Lecturer { get; set; }    // Additional course info
    }
}
