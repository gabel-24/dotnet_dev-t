using Microsoft.AspNetCore.Identity;

namespace JobApplicationApi.Services;

public sealed class RegistrationException : Exception
{
    public Dictionary<string, string[]> Errors { get; }
    public bool IsDuplicate { get; }

    public RegistrationException(IEnumerable<IdentityError> errors)
        : base("Please correct the registration details and try again.")
    {
        var failures = errors.ToArray();
        IsDuplicate = failures.Any(e => e.Code is "DuplicateEmail" or "DuplicateUserName");
        Errors = failures.GroupBy(e => e.Code.Contains("Email") ? "Email"
                : e.Code.Contains("UserName") ? "Username"
                : e.Code.StartsWith("Password") ? "Password" : "Registration")
            .ToDictionary(group => group.Key, group => group.Select(e => e.Code switch
            {
                "DuplicateEmail" => "An account with this email already exists. Please log in or use another email.",
                "DuplicateUserName" => "This username is already taken. Please choose another username.",
                _ => e.Description
            }).Distinct().ToArray());
    }
}
