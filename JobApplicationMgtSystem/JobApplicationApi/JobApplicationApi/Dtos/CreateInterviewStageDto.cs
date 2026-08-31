using JobApplicationApi.Models;

namespace JobApplicationApi.Dtos
{
    public class CreateInterviewStageDto
    {
        public string StageName { get; set; } = string.Empty;
        public InterviewType Type { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Interviewer { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateOnly ScheduledAt { get; set; }
    }
}
