namespace JobApplicationApi.Dtos
{
    public class InterviewStageSummaryDto
    {
        public int Id { get; set; }
        public string StageName { get; set; } = string.Empty;
        public DateOnly ScheduledAt { get; set; }
        public bool IsComplete { get; set; } = false;
    }
}
