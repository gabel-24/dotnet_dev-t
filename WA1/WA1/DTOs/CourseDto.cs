namespace WA1.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Duration { get; set; }
        public string? Lecturer { get; set; }
        public int Fees { get; set; }
        public int StudentCount { get; set; }  // Additional info
    }
}
