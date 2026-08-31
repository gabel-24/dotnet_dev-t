using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WA1.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHarsh { get; set; } = string.Empty;
        public string Role {  get; set; } = "User";

    }
}
