using JobApplicationApi.Data;
using JobApplicationApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationApi.Repositories
{
    public class JobPostingRepository : IJobPostingRepository
    {
        private readonly AppDbContext _context;

        public JobPostingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<JobPosting?> GetByIdAsync(int id)
        {
            return await _context.JobPostings.FindAsync(id);
        }
        public async Task<(List<JobPosting> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? location, string? employmentType, string? keyword)
        {
            var query = _context.JobPostings
                .Include(j => j.Recruiter)
                .Where(j => j.IsActive);

            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(j => j.Location != null && j.Location.Contains(location));
            }

            if (!string.IsNullOrWhiteSpace(employmentType))
            {
                query = query.Where(j => j.EmploymentType != null && j.EmploymentType.Contains(employmentType));
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(j => j.Title.Contains(keyword) || j.Description.Contains(keyword));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(j => j.PostedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
        public async Task<List<JobPosting>> GetByRecruiterIdAsync(int recruiterId)
        {
            return await _context.JobPostings.Where(j => j.RecruiterId == recruiterId).ToListAsync();
        }
        public async Task AddAsync(JobPosting jobPosting)
        {
            _context.JobPostings.Add(jobPosting);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(JobPosting jobPosting)
        {
            _context.JobPostings.Update(jobPosting);
            await _context.SaveChangesAsync();
        }
    }
}
