using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required!")]
        public string Name { get; set; }
    }
}