namespace JobApplicationApi.Models
{
    public class Candidate
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public string? ResumeUrl { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}
