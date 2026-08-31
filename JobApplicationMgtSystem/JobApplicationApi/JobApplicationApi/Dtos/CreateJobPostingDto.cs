using System.ComponentModel.DataAnnotations;

namespace JobApplicationApi.Dtos
{
    public class CreateJobPostingDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string EmploymentType { get; set; } = string.Empty;
        public int SalaryMin {  get; set; }
        public int SalaryMax {  get; set; }
        public DateOnly ClosingDate {  get; set; }
    }
}
