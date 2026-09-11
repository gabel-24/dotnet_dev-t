using JobApplicationApi.Data;
using JobApplicationApi.Models;
using JobApplicationApi.Repositories;
using JobApplicationApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// CORS origins come from config (Cors:AllowedOrigins), a comma-separated list.
// Local dev: appsettings.Development.json -> "http://localhost:5173"
// Production: set via the Cors__AllowedOrigins app setting in Azure, e.g.
// "https://<your-static-web-app>.azurestaticapps.net"
var allowedOrigins = (builder.Configuration["Cors:AllowedOrigins"] ?? string.Empty)
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
options.AddPolicy("AllowFrontend", policy =>
    {
policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
}); 

// Azure App Service terminates HTTPS at its edge and forwards plain HTTP to
// the app internally. Without this, the app can misjudge the request scheme
// and UseHttpsRedirection/UseHsts can cause redirect loops in production.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Azure's load balancer sits in front of the app, so trust its forwarded
    // headers by clearing the default known-network restrictions.
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
options.Password.RequiredLength = 6;
options.Password.RequireNonAlphanumeric = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
};
});

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<IRecruiterRepository, RecruiterRepository>();
builder.Services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
builder.Services.AddScoped<IJobPostingRepository, JobPostingRepository>();
builder.Services.AddScoped<IInterviewStageRepository, InterviewStageRepository>();


builder .Services.AddScoped<ICandidateService, CandidateService>();
builder .Services.AddScoped<IRecruiterService, RecruiterService>();
builder .Services.AddScoped<IInterviewStageService, InterviewStageService>();
builder .Services.AddScoped<IJobApplicationService, JobApplicationService>();
builder .Services.AddScoped<IJobPostingService, JobPostingService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder .Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Must run before UseHttpsRedirection/UseHsts so the app sees the original
// scheme from Azure's edge instead of the internal http hop.
app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Admin", "Recruiter", "Candidate" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        await roleManager.CreateAsync(new IdentityRole(role));
    }
}

app.Run();

public partial class Program { }