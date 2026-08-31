using JobApplicationApi.Data;
using JobApplicationApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationApi.Repositories
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly AppDbContext _context;

        public JobApplicationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<JobApplication?> GetByIdAsync(int id)
        {
            return await _context.JobApplications
                .Include(a => a.Candidate).ThenInclude(c => c.User)
                .Include(a => a.JobPosting).ThenInclude(jp => jp.Recruiter)
                .Include(a => a.InterviewStages)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<bool> ExistsAsync(int candidateId, int jobPostingId)
        {
            return await _context.JobApplications
                .AnyAsync(a => a.CandidateProfileId == candidateId && a.JobPostingId == jobPostingId);
        }
        public async Task<(List<JobApplication> Items, int TotalCount)> GetByCandidateIdAsync(int candidateId, int pageNumber, int pageSize)
        {
            var query = _context.JobApplications
                .Include(a => a.Candidate).ThenInclude(c => c.User)
                .Include(a => a.JobPosting)
                .Where(a => a.CandidateProfileId == candidateId);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(a => a.AppliedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(List<JobApplication> Items, int TotalCount)> GetByJobPostingIdAsync(int jobPostingId, int pageNumber, int pageSize)
        {
            var query = _context.JobApplications
                .Include(a => a.Candidate).ThenInclude(c => c.User)
                .Include(a => a.JobPosting)
                .Where(a => a.JobPostingId == jobPostingId);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(a => a.AppliedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
        public async Task AddAsync(JobApplication jobApplication)
        {
            _context.JobApplications.Add(jobApplication);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(JobApplication jobApplication)
        {
            _context.JobApplications.Update(jobApplication);
            await _context.SaveChangesAsync();
        }
    }
}
