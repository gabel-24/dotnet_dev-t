using AutoMapper;
using JobApplicationApi.Dtos;
using JobApplicationApi.Models;
using JobApplicationApi.Repositories;

namespace JobApplicationApi.Services
{
    public class InterviewStageService : IInterviewStageService
    {
        private readonly IInterviewStageRepository _repository;
        private readonly IJobApplicationRepository _jarepository;
        private readonly IMapper _mapper;

        public InterviewStageService(IInterviewStageRepository repository,IJobApplicationRepository jarepository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _jarepository = jarepository;
        }

        public async Task<List<InterviewStageDto>> GetByJobApplicationIdAsync(int jobApplicationId, string userId, bool isRecruiter)
        {
            var application = await _jarepository.GetByIdAsync(jobApplicationId);

            if (application == null)
                return new List<InterviewStageDto>();

            if (isRecruiter)
            {
                if (application.JobPosting.Recruiter.UserId != userId)
                    return new List<InterviewStageDto>();
            }
            else
            {
                if (application.Candidate.UserId != userId)
                    return new List<InterviewStageDto>();
            }

            var stages = await _repository.GetByJobApplicationIdAsync(jobApplicationId);
            var stageDtos = _mapper.Map<List<InterviewStageDto>>(stages);

            if (!isRecruiter)
            {
                foreach (var dto in stageDtos)
                {
                    dto.Notes = null;
                }
            }

            return stageDtos;
        }
        public async Task<InterviewStageDto?> CreateAsync(int recruiterId, int jobApplicationId, CreateInterviewStageDto request)
        {
            var application = await _jarepository.GetByIdAsync(jobApplicationId);

            if (application == null)
                return null;

            if (application.JobPosting.RecruiterId != recruiterId)
                return null;

            var interviewStage = _mapper.Map<InterviewStage>(request);
            interviewStage.JobApplicationId = jobApplicationId;

            await _repository.AddAsync(interviewStage);

            return _mapper.Map<InterviewStageDto>(interviewStage);
        }
        public async Task<bool> UpdateAsync(int recruiterId, int interviewStageId, UpdateInterviewStageDto request)
        {
            var interviewStage = await _repository.GetByIdAsync(interviewStageId);

            if(interviewStage == null)
            {
                return false;
            }

            if(interviewStage.JobApplication.JobPosting.RecruiterId != recruiterId)
            {
                return false;
            }

            _mapper.Map(request, interviewStage);
            await _repository.UpdateAsync(interviewStage);
            return true;
        }

        public async Task<InterviewStageDto?> GetByIdAsync(int interviewStageId)
        {
            var interviewStage = await _repository.GetByIdAsync(interviewStageId);

            return interviewStage == null ? null : _mapper.Map<InterviewStageDto>(interviewStage);
        }

        public async Task<bool> DeleteAsync(int recruiterId, int interviewStageId)
        {
            var interviewStage = await _repository.GetByIdAsync(interviewStageId);

            if(interviewStage == null)
            {
                return false;
            }

            if(interviewStage.JobApplication.JobPosting.RecruiterId != recruiterId)
            {
                return false;
            }

            await _repository.DeleteAsync(interviewStage);
            return true;
        }
    }
}
