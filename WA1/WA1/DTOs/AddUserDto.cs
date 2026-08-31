namespace WA1.DTOs
{
    public class AddUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password {  get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
