using Microsoft.AspNetCore.Identity;

namespace JobApplicationApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        // IdentityUser already provides: Id, UserName, Email, PasswordHash, PhoneNumber, etc.
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public Candidate? CandidateProfile { get; set; }
        public Recruiter? RecruiterProfile { get; set; }
    }
}