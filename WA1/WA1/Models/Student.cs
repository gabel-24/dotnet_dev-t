using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WA1.Models
{
    public class Student
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public int CourseId {  get; set; }
        [Required]
        public Course? Course {  get; set; }
        [Range(16,100)]
        public int Age {  get; set; }
        [EmailAddress]
        public string? Email {  get; set; }
        public int UserId { get; set; } // new
        public User User { get; set; }

    }
}
