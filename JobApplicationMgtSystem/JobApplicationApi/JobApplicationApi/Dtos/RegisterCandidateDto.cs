using System.ComponentModel.DataAnnotations;

namespace JobApplicationApi.Dtos
{
    public class RegisterCandidateDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
        public string Headline { get; set; } = string.Empty;
        public string ResumeUrl { get; set; } = string.Empty;
        public List<string> Skills { get; set; } = new List<string>();
    }
}
