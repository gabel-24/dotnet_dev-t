namespace JobApplicationApi.Dtos
{
    public class ErrorDto
    {
        public int StatusCode {  get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string>? Errors { get; set; }
    }
}
