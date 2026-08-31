using JobApplicationApi.Models;

namespace JobApplicationApi.Dtos
{
    public class JobApplicationDto
    {
        public int Id { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateOnly AppliedAt { get; set; }
        public string CoverLetter {  get; set; } = string.Empty;
        public string ResumeSnapshotUrl {  get; set; } = string.Empty;
        public CandidateSummaryDto Candidate { get; set; } = null!;
        public JobPostingSummaryDto JobPosting { get; set; } = null!;
        public List<InterviewStageDto> InterviewStages { get; set; } = new List<InterviewStageDto>();
    }
}
