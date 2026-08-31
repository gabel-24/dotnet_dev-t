using JobApplicationApi.Models;

namespace JobApplicationApi.Dtos
{
    public class InterviewStageDto
    {
        public int Id { get; set; }
        public string StageName { get; set; } = string.Empty;
        public InterviewType Type { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Interviewer { get; set; } = string.Empty;
        public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;
        public DateOnly ScheduledAt { get; set; }
        public string Notes { get; set; } = string.Empty;
        public bool IsComplete { get; set; } = false;
    }
    
}
