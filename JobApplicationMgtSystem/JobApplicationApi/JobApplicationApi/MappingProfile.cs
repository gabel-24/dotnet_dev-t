using AutoMapper;
using JobApplicationApi.Dtos;
using JobApplicationApi.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ===== Candidate =====
        CreateMap<Candidate, CandidateDto>();
        CreateMap<Candidate, CandidateSummaryDto>();
        CreateMap<RegisterCandidateDto, Candidate>();
        CreateMap<UpdateCandidateDto, Candidate>();

        // ===== Recruiter =====
        CreateMap<Recruiter, RecruiterDto>();
        CreateMap<Recruiter, RecruiterSummaryDto>();
        CreateMap<RegisterRecruiterDto, Recruiter>();
        CreateMap<UpdateRecruiterDto, Recruiter>();

        // ===== JobPosting =====
        CreateMap<JobPosting, JobPostingDto>()
            .ForMember(dest => dest.ApplicationCount,
                       opt => opt.MapFrom(src => src.Applications.Count));
        CreateMap<JobPosting, JobPostingSummaryDto>();
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

        // ===== InterviewStage =====
        CreateMap<InterviewStage, InterviewStageDto>();
        CreateMap<CreateInterviewStageDto, InterviewStage>();
        CreateMap<UpdateInterviewStageDto, InterviewStage>();
    }
}