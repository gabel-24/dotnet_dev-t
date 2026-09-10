using AutoMapper;
using JobApplicationApi.Dtos;
using JobApplicationApi.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ===== Candidate =====
        CreateMap<Candidate, CandidateDto>()
            .ForMember(d => d.Username, o => o.MapFrom(s => s.User.UserName))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.User.Email));
        CreateMap<Candidate, CandidateSummaryDto>()
            .ForMember(d => d.Username, o => o.MapFrom(s => s.User.UserName));
        CreateMap<RegisterCandidateDto, Candidate>();
        CreateMap<UpdateCandidateDto, Candidate>();

        // ===== Recruiter =====
        CreateMap<Recruiter, RecruiterDto>()
            .ForMember(d => d.Username, o => o.MapFrom(s => s.User.UserName))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.User.Email));
        CreateMap<Recruiter, RecruiterSummaryDto>()
            .ForMember(d => d.Username, o => o.MapFrom(s => s.User.UserName));
        CreateMap<RegisterRecruiterDto, Recruiter>();
        CreateMap<UpdateRecruiterDto, Recruiter>();

        // ===== JobPosting =====
        CreateMap<JobPosting, JobPostingDto>()
            .ForMember(dest => dest.ApplicationCount,
                       opt => opt.MapFrom(src => src.Applications.Count));
        CreateMap<JobPosting, JobPostingSummaryDto>()
            .ForMember(dest => dest.ApplicationCount,
                        opt => opt.MapFrom(src => src.Applications.Count));
        CreateMap<CreateJobPostingDto, JobPosting>();
        CreateMap<UpdateJobPostingDto, JobPosting>();

        // ===== JobApplication =====
        CreateMap<JobApplication, JobApplicationDto>();
        CreateMap<JobApplication, JobApplicationSummaryDto>()
            .ForMember(dest => dest.CandidateName,
                       opt => opt.MapFrom(src => src.Candidate.User.UserName))
            .ForMember(dest => dest.JobTitle,
                       opt => opt.MapFrom(src => src.JobPosting.Title));
        CreateMap<CreateJobApplicationDto, JobApplication>();
        CreateMap<UpdateJobApplicationDto, JobApplication>();

        // ===== InterviewStage =====
        CreateMap<InterviewStage, InterviewStageDto>();
        CreateMap<CreateInterviewStageDto, InterviewStage>();
        CreateMap<UpdateInterviewStageDto, InterviewStage>();
    }
}