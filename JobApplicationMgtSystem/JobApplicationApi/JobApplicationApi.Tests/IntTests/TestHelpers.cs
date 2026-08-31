using Microsoft.AspNetCore.Identity.Data;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using JobApplicationApi.Dtos;

public static class TestHelpers
{
    public static async Task<string> RegisterAndLoginCandidateAsync(HttpClient client, string email, string userName)
    {
        var registerRequest = new RegisterCandidateDto
        {
            Email = email,
            Password = "Password123",
            Username = userName,
            Skills = new List<string> { "C#" }
        };

        await client.PostAsJsonAsync("/api/auth/register/candidate", registerRequest);

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = email,
            Password = "Password123"
        });

        var result = await loginResponse.Content.ReadFromJsonAsync<AuthDto>();
        return result!.Token;
    }

    public static async Task<string> RegisterAndLoginRecruiterAsync(HttpClient client, string email, string userName)
    {
        var registerRequest = new RegisterRecruiterDto
        {
            Email = email,
            Password = "Password123",
            Username = userName,
            CompanyName = "TestCo"
        };

        await client.PostAsJsonAsync("/api/auth/register/recruiter", registerRequest);

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = email,
            Password = "Password123"
        });

        var result = await loginResponse.Content.ReadFromJsonAsync<AuthDto>();
        return result!.Token;
    }

    public static void AttachToken(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}