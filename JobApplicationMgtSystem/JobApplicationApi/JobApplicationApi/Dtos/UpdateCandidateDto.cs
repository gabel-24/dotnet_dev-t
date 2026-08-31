namespace JobApplicationApi.Dtos
{
    public class UpdateCandidateDto
    {
        public string Username { get; set; } = string.Empty;
        public string Headline { get; set; } = string.Empty;
        public string ResumeUrl { get; set; } = string.Empty;
        public List<string> Skills { get; set; } = new List<string>();
    }
}
