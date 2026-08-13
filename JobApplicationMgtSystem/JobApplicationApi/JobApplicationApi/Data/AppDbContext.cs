using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using JobApplicationApi.Models;

namespace JobApplicationApi.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Recruiter> Recruiters { get; set; }
        public DbSet<JobPosting> JobPostings { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<InterviewStage> InterviewStages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // required — sets up Identity tables

            // One-to-one: ApplicationUser <-> Candidate
            builder.Entity<Candidate>()
                .HasOne(c => c.User)
                .WithOne(u => u.CandidateProfile)
                .HasForeignKey<Candidate>(c => c.UserId);

            // One-to-one: ApplicationUser <-> Recruiter
            builder.Entity<Recruiter>()
                .HasOne(r => r.User)
                .WithOne(u => u.RecruiterProfile)
                .HasForeignKey<Recruiter>(r => r.UserId);

            // Prevent a candidate applying twice to the same job
            builder.Entity<JobApplication>()
                .HasIndex(a => new { a.CandidateProfileId, a.JobPostingId })
                .IsUnique();
        }
    }
}