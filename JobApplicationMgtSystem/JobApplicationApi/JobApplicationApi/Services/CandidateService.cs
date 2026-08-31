using AutoMapper;
using JobApplicationApi.Repositories;
using JobApplicationApi.Services;
using JobApplicationApi.Dtos;

public class CandidateService : ICandidateService
{
    private readonly ICandidateRepository _repository;
    private readonly IMapper _mapper;

    public CandidateService(ICandidateRepository candidateRepository, IMapper mapper)
    {
        _repository = candidateRepository;
        _mapper = mapper;
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

        _mapper.Map(request, candidate);
        await _repository.UpdateInfoAsync(candidate);

        return true;
    }
}