using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities
{
    public class Document

    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name {  get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public byte[] File { get; set; }
        public Customer Customer { get; set; }
    }
}
