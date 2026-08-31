using JobApplicationApi.Models;

namespace JobApplicationApi.Repositories
{
    public interface IInterviewStageRepository
    {
        Task<InterviewStage?> GetByIdAsync(int id);
        Task<List<InterviewStage>> GetByJobApplicationIdAsync(int jobApplicationId);
        Task AddAsync(InterviewStage interviewStage);
        Task UpdateAsync(InterviewStage interviewStage);
        Task DeleteAsync(InterviewStage interviewStage);
    }
}
