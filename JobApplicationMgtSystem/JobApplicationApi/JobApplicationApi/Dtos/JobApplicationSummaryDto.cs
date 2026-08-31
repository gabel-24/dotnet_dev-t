using JobApplicationApi.Models;

namespace JobApplicationApi.Dtos
{
    public class JobApplicationSummaryDto
    {
        public int Id { get; set; }
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Submitted;
        public DateOnly AppliedAt { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string JobTitle { get; set; }=string.Empty;
    }
}
