using AutoMapper;
using JobApplicationApi.Dtos;
using JobApplicationApi.Models;
using JobApplicationApi.Repositories;
using Microsoft.AspNetCore.Identity;

namespace JobApplicationApi.Services
{
    public class RecruiterService : IRecruiterService
    {
        private readonly IRecruiterRepository _repository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public RecruiterService(IRecruiterRepository repository, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _mapper = mapper;
            _userManager = userManager;
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

            recruiter.CompanyName = request.CompanyName;

            if(recruiter.User.UserName != request.Username)
            {
                var result = await _userManager.SetUserNameAsync(recruiter.User, request.Username);

                if(!result.Succeeded) return false;
            }

            await _repository.UpdateAsync(recruiter);

            return true;
        }
    }
}
