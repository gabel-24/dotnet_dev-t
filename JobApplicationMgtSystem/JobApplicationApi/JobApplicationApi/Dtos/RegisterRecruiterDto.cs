using System.ComponentModel.DataAnnotations;

namespace JobApplicationApi.Dtos
{
    public class RegisterRecruiterDto
    {
        [Required]
        public string Username {  get; set; } = string.Empty;
        [EmailAddress]
        public string Email {  get; set; }    = string.Empty;
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string CompanyName {  get; set; } =string.Empty;
        public string Department { get; set; } = string.Empty;
    }
}
