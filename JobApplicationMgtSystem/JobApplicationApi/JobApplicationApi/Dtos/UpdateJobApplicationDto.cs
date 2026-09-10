using JobApplicationApi.Models;

namespace JobApplicationApi.Dtos
{
    public class UpdateJobApplicationDto
    {
        [System.ComponentModel.DataAnnotations.EnumDataType(typeof(ApplicationStatus))]
        public ApplicationStatus Status { get; set; }
    }
}
