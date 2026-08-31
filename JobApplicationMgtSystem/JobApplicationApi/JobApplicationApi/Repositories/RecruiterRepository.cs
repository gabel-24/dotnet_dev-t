using JobApplicationApi.Data;
using JobApplicationApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationApi.Repositories
{
    public class RecruiterRepository : IRecruiterRepository
    {
        private readonly AppDbContext _context;

        public RecruiterRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Recruiter?> GetByIdAsync(int id)
        {
            return await _context.Recruiters
                            .Include(r =>r.User)
                            .FirstOrDefaultAsync(r => r.Id == id);
        }
        public async Task<Recruiter?> GetByUserIdAsync(string userId)
        {
            return await _context.Recruiters
                                .Include (r => r.User)
                                .FirstOrDefaultAsync (r => r.UserId == userId);
        }
        public async Task UpdateAsync(Recruiter recruiter)
        {
            _context.Recruiters.Update(recruiter);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(Recruiter recruiter)
        {
            _context.Recruiters.Add(recruiter);
            await _context.SaveChangesAsync();
        }
    }
}
