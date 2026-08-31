namespace JobApplicationApi.Dtos
{
    public class JobPostingDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string EmploymentType { get; set; } = string.Empty;
        public int SalaryMin {  get; set; }
        public int SalaryMax {  get; set; }
        public DateOnly ClosingDate {  get; set; }
        public bool IsActive { get; set; } = true;
        public RecruiterSummaryDto Recruiter { get; set; } = null!;
        public int ApplicationCount;
    }
}
