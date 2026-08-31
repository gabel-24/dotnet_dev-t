using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WA1.Models
{
    public class Course
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }
        [Required]
        public string? Name {  get; set; }
        [Range(1, 5)]
        public int Duration {  get; set; }
        [Required]
        public string? Lecturer {  get; set; }

        public int Fees {  get; set; }
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
