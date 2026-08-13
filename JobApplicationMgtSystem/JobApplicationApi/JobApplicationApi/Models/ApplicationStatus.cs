namespace JobApplicationApi.Models
{
    public enum ApplicationStatus
    {
        Submitted,
        Interview,
        Offer,
        Rejected,
        Withdrawn
    }

    public class InterviewStage
    {
        public int Id { get; set; }
        public string StageName { get; set; } = string.Empty;  // "Phone screen", "Technical", "Final"
        public DateTime? ScheduledAt { get; set; }
        public string? Notes { get; set; }
        public bool IsCompleted { get; set; } = false;

        public int JobApplicationId { get; set; }
        public JobApplication JobApplication { get; set; } = null!;
    }
}
