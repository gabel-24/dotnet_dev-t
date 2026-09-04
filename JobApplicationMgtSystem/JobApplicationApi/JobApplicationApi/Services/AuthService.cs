using AutoMapper;
using JobApplicationApi.Dtos;
using JobApplicationApi.Models;
using JobApplicationApi.Repositories;
using JobApplicationApi.Services;
using Microsoft.AspNetCore.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ICandidateRepository _candidateRepository;
    private readonly IRecruiterRepository _recruiterRepository;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ICandidateRepository candidateRepository,
        IRecruiterRepository recruiterRepository,
        ITokenService tokenService,
        IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _candidateRepository = candidateRepository;
        _recruiterRepository = recruiterRepository;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<AuthDto?> RegisterCandidateAsync(RegisterCandidateDto request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Identity error: {error.Code} - {error.Description}");
            }
            return null;
            
        }
            

        await _userManager.AddToRoleAsync(user, "Candidate");

        var candidate = new Candidate
        {
            UserId = user.Id,
            ResumeUrl = request.ResumeUrl,
            Skills = request.Skills
        };

        await _candidateRepository.AddAsync(candidate);

        var token = _tokenService.GenerateToken(user, "Candidate");

        return new AuthDto
        {
            Token = token,
            UserId = user.Id,
            Role = "Candidate",
            UserName = user.UserName
        };
    }

    public async Task<AuthDto?> RegisterRecruiterAsync(RegisterRecruiterDto request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return null;

        await _userManager.AddToRoleAsync(user, "Recruiter");

        var recruiter = new Recruiter
        {
            UserId = user.Id,
            CompanyName = request.CompanyName
        };

        await _recruiterRepository.AddAsync(recruiter);

        var token = _tokenService.GenerateToken(user, "Recruiter");

        return new AuthDto
        {
            Token = token,
            UserId = user.Id,
            Role = "Recruiter",
            UserName = user.UserName
        };
    }

    public async Task<AuthDto?> LoginAsync(LoginDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return null;

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

        if (!result.Succeeded)
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? string.Empty;

        var token = _tokenService.GenerateToken(user, role);

        return new AuthDto
        {
            Token = token,
            UserId = user.Id,
            Role = role,
            UserName = user.UserName
        };
    }
}
