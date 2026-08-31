using JobApplicationApi.Dtos;

namespace JobApplicationApi.Services
{
    public interface IAuthService
    {
        Task<AuthDto?> RegisterCandidateAsync(RegisterCandidateDto request);
        Task<AuthDto?> RegisterRecruiterAsync(RegisterRecruiterDto request);
        Task<AuthDto?> LoginAsync(LoginDto request);
    }
}
