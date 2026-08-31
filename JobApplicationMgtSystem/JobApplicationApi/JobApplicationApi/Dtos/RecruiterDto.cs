namespace JobApplicationApi.Dtos
{
    public class RecruiterDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }
}
