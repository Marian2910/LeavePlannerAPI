using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities
{
    public class Job
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required!")]
        public string Title { get; set; }
        public string Role { get; set; }
    }
}