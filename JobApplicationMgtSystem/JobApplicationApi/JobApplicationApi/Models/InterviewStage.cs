namespace JobApplicationApi.Models
{
    public class InterviewStage
    {
        public int Id { get; set; }
        public string StageName { get; set; } = string.Empty;  // "Phone screen", "Technical", "Final"
        public InterviewType Type { get; set; }
        public DateOnly? ScheduledAt { get; set; }
        public string? Location { get; set; }
        public string? Interviewer { get; set; }
        public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;
        public string? Notes { get; set; }

        public int JobApplicationId { get; set; }
        public JobApplication JobApplication { get; set; } = null!;
    }

    public enum InterviewType
    {
        Phone,
        Video,
        OnSite,
        Technical,
        Behavioral,
        Final
    }

    public enum InterviewStatus
    {
        Scheduled,
        Completed,
        Cancelled,
        Rescheduled,
        Passed,
        Failed
    }
}