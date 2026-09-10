using AutoMapper;
using JobApplicationApi.Repositories;
using JobApplicationApi.Services;
using JobApplicationApi.Dtos;

public class CandidateService : ICandidateService
{
    private readonly ICandidateRepository _repository;
    private readonly IMapper _mapper;
    private readonly Microsoft.AspNetCore.Identity.UserManager<JobApplicationApi.Models.ApplicationUser> _userManager;

    public CandidateService(ICandidateRepository candidateRepository, IMapper mapper, Microsoft.AspNetCore.Identity.UserManager<JobApplicationApi.Models.ApplicationUser> userManager)
    {
        _repository = candidateRepository;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<CandidateDto?> GetByIdAsync(int id)
    {
        var candidate = await _repository.GetByIdAsync(id);
        return candidate == null ? null : _mapper.Map<CandidateDto>(candidate);
    }

    public async Task<CandidateDto?> GetByUserIdAsync(string userId)
    {
        var candidate = await _repository.GetByUserIdAsync(userId);
        return candidate == null ? null : _mapper.Map<CandidateDto>(candidate);
    }

    public async Task<bool> UpdateAsync(string userId, UpdateCandidateDto request)
    {
        var candidate = await _repository.GetByUserIdAsync(userId);

        if (candidate == null)
            return false;

        if (candidate.User.UserName != request.Username)
        {
            var result = await _userManager.SetUserNameAsync(candidate.User, request.Username);
            if (!result.Succeeded)
                throw new BadHttpRequestException(string.Join(" ", result.Errors.Select(e => e.Description)));
        }
        _mapper.Map(request, candidate);
        await _repository.UpdateInfoAsync(candidate);

        return true;
    }
}