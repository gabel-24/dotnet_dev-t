using JobApplicationApi.Data;
using JobApplicationApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationApi.Repositories
{
    public class InterviewStageRepository : IInterviewStageRepository
    {
        private readonly AppDbContext _context;

        public InterviewStageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<InterviewStage?> GetByIdAsync(int id)
        {
            return await _context.InterviewStages
                .Include(i => i.JobApplication).ThenInclude(a => a.JobPosting)
                .FirstOrDefaultAsync(i => i.Id == id);
        }
        public async Task<List<InterviewStage>> GetByJobApplicationIdAsync(int jobApplicationId)
        {
            return await _context.InterviewStages.Where(i => i.JobApplicationId == jobApplicationId).OrderBy(i => i.ScheduledAt).ToListAsync();
        }
        public async Task AddAsync(InterviewStage interviewStage)
        {
            _context.InterviewStages.Add(interviewStage);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(InterviewStage interviewStage)
        {
            _context.InterviewStages.Update(interviewStage);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(InterviewStage interviewStage)
        {
            _context.InterviewStages.Remove(interviewStage);
            await _context.SaveChangesAsync();
        }
    }
}
