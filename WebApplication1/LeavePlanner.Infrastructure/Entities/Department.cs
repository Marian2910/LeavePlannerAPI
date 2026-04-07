using System.ComponentModel.DataAnnotations;

namespace LeavePlanner.Infrastructure.Entities
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Name is required!")]
        [MaxLength(50)]
        public required string Name { get; set; }
    }
}