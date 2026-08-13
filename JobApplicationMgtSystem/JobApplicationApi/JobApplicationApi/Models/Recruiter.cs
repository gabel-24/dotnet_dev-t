namespace JobApplicationApi.Models
{
    public class Recruiter
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public string? CompanyName { get; set; }
        public ICollection<JobPosting> PostedJobs { get; set; } = new List<JobPosting>();
    }
}
