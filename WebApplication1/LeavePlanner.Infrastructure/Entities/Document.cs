using System.ComponentModel.DataAnnotations;

namespace LeavePlanner.Infrastructure.Entities
{
    public class Document
    {
        [Key]
        public int Id { get; init; }

        [Required(ErrorMessage = "Document name is required.")]
        [MaxLength(150)]
        public string Name { get; init; } = string.Empty;

        [MaxLength(50)]
        public string? Type { get; init; }

        public DateTime CreatedAt { get; init; }

        [Required]
        public byte[] File { get; init; } = [];

        public int CustomerId { get; init; }

        public Customer Customer { get; init; } = null!;
    }
}
