namespace JobApplicationApi.Models
{
    public class JobApplication
    {
        public int Id { get; set; }
        public DateOnly AppliedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Submitted;
        public string? CoverLetter { get; set; }
        public string? ResumeSnapshotUrl { get; set; }  // resume as it was at time of applying

        // FK to candidate
        public int CandidateProfileId { get; set; }
        public Candidate Candidate { get; set; } = null!;

        // FK to the job they applied for
        public int JobPostingId { get; set; }
        public JobPosting JobPosting { get; set; } = null!;
        public ICollection<InterviewStage> InterviewStages { get; set; } = new List<InterviewStage>();
    }
}
