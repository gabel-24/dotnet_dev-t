using JobApplicationApi.Models;

namespace JobApplicationApi.Dtos
{
    public class UpdateInterviewStageDto
    {
        public string StageName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;
        public InterviewType Type { get; set; } 
        public DateOnly ScheduledAt { get; set; }
        public string Notes { get; set; } = string.Empty;
        public bool IsComplete {  get; set; } = false;
    }
}
