namespace WA1.DTOs
{
    public class StudentQueryParams
    {
        public string? Search { get; set; }       // matches against Name or Email
        public int? CourseId { get; set; }         // optional filter
        public string SortBy { get; set; } = "Name"; // which field to sort by
        public bool Descending { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
