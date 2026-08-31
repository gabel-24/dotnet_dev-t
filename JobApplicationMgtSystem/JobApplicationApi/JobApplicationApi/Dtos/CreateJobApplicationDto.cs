using System.ComponentModel.DataAnnotations;

namespace JobApplicationApi.Dtos
{
    public class CreateJobApplicationDto
    {
        [Required]
        public int JobPostingId {  get; set; }
        public string CoverLetter { get; set; } = string.Empty;
    }
}
