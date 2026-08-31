using AutoMapper;
using JobApplicationApi.Dtos;
using JobApplicationApi.Repositories;

namespace JobApplicationApi.Services
{
    public class RecruiterService : IRecruiterService
    {
        private readonly IRecruiterRepository _repository;
        private readonly IMapper _mapper;

        public RecruiterService(IRecruiterRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<RecruiterDto?> GetByIdAsync(int id)
        {
            var recruiter = await _repository.GetByIdAsync(id);
            return recruiter == null ? null : _mapper.Map<RecruiterDto>(recruiter);
        }
        public async Task<RecruiterDto?> GetByUserIdAsync(string userId)
        {
            var recruiter = await _repository.GetByUserIdAsync(userId);
            return recruiter == null ? null : _mapper.Map<RecruiterDto>(recruiter);
        }
        public async Task<bool> UpdateAsync(string userId, UpdateRecruiterDto request)
        {
            var recruiter = await _repository.GetByUserIdAsync(userId);

            if(recruiter == null)
            {
                return false;
            }

            _mapper.Map(request, recruiter);
            await _repository.UpdateAsync(recruiter);

            return true;
        }
    }
}
