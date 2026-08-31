namespace JobApplicationApi.Models
{
    public class JobPosting
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? EmploymentType { get; set; }   // Full-time, Part-time, Contract, etc.
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public DateOnly PostedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public DateOnly? ClosingDate { get; set; }
        public bool IsActive { get; set; } = true;

        // FK back to the recruiter who posted it
        public int RecruiterId { get; set; }
        public Recruiter Recruiter { get; set; } = null!;

        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}
