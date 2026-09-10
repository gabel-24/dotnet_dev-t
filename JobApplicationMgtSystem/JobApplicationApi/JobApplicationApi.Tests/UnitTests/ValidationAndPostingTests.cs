using AutoMapper;
using JobApplicationApi.Data;
using JobApplicationApi.Dtos;
using JobApplicationApi.Models;
using JobApplicationApi.Repositories;
using JobApplicationApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

[Trait("Category", "Unit")]
public class ValidationAndPostingTests
{
    private static IMapper Mapper() => new MapperConfiguration(c => c.AddProfile<MappingProfile>(), NullLoggerFactory.Instance).CreateMapper();

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    [InlineData(1, -1)]
    [InlineData(1, 101)]
    [InlineData(int.MaxValue, 100)]
    public async Task InvalidPagination_IsRejectedBeforeQuerying(int page, int size)
    {
        var repo = new Mock<IJobPostingRepository>(MockBehavior.Strict);
        var service = new JobPostingService(repo.Object, Mapper());
        var error = await Assert.ThrowsAsync<BadHttpRequestException>(() => service.GetPagedAsync(page, size, null, null, null));
        Assert.Equal(400, error.StatusCode);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(999)]
    public async Task UndefinedStatus_IsRejected(int value)
    {
        var request = new UpdateJobApplicationDto { Status = (ApplicationStatus)value };
        Assert.False(Validator.TryValidateObject(request, new ValidationContext(request), new List<ValidationResult>(), true));
        var service = new JobApplicationService(new Mock<IJobApplicationRepository>(MockBehavior.Strict).Object,
            new Mock<IJobPostingRepository>().Object, new Mock<ICandidateRepository>().Object, Mapper());
        await Assert.ThrowsAsync<BadHttpRequestException>(() => service.UpdateStatusAsync(1, 1, request));
    }

    [Fact]
    public async Task Browse_ExcludesExpiredAndInactiveJobs_AndKeepsUndatedJobs()
    {
        await using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var recruiter = new Recruiter { User = new ApplicationUser { UserName = "recruiter" } };
        db.JobPostings.AddRange(
            new JobPosting { Title = "Expired", ClosingDate = today.AddDays(-1), Recruiter = recruiter },
            new JobPosting { Title = "Inactive", IsActive = false, Recruiter = recruiter },
            new JobPosting { Title = "Today", ClosingDate = today, Recruiter = recruiter },
            new JobPosting { Title = "Undated", Recruiter = recruiter });
        await db.SaveChangesAsync();
        var (items, count) = await new JobPostingRepository(db).GetPagedAsync(1, 10, null, null, null);
        Assert.Equal(2, count);
        Assert.Equal(new[] { "Today", "Undated" }, items.Select(i => i.Title).OrderBy(t => t));
    }

    [Fact]
    public void OmittedClosingDate_RemainsNullThroughMappingAndSerialization()
    {
        var request = JsonSerializer.Deserialize<CreateJobPostingDto>("{\"Title\":\"Developer\"}")!;
        var posting = Mapper().Map<JobPosting>(request);
        Assert.Null(posting.ClosingDate);
        Assert.Null(Mapper().Map<JobPostingDto>(posting).ClosingDate);
    }

    [Fact]
    public async Task RegisterCandidate_PersistsHeadline()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
        await using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var sp = scope.ServiceProvider;
        await sp.GetRequiredService<RoleManager<IdentityRole>>().CreateAsync(new IdentityRole("Candidate"));
        var candidates = new Mock<ICandidateRepository>();
        var service = new AuthService(sp.GetRequiredService<UserManager<ApplicationUser>>(),
            sp.GetRequiredService<SignInManager<ApplicationUser>>(), candidates.Object,
            new Mock<IRecruiterRepository>().Object, new Mock<ITokenService>().Object, Mapper());
        var result = await service.RegisterCandidateAsync(new RegisterCandidateDto
        {
            Username = "headlineuser", Email = "headline@example.com", Password = "StrongPass1!", Headline = "Backend developer"
        });
        Assert.NotNull(result);
        candidates.Verify(r => r.AddAsync(It.Is<Candidate>(c => c.Headline == "Backend developer")), Times.Once);
    }
}
