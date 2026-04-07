using System.ComponentModel.DataAnnotations;

namespace LeavePlanner.Infrastructure.Entities
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Customer name is required!")]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Customer email is required!")]
        [EmailAddress(ErrorMessage = "Email not valid!")]
        [MaxLength(150)]
        public required string Email { get; set; }

        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        [MaxLength(150)]
        public string Street { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Number { get; set; } = string.Empty;

        public bool Status { get; set; } = true;

        [MaxLength(20)]
        public string BillingType { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "TVA must be between 0 and 100.")]
        public int Tva { get; set; }

        [MaxLength(250)]
        public string Addition { get; set; } = string.Empty;

        // Explicitly initializing the collection to avoid null reference warnings.
        public ICollection<Document> Documents { get; set; } = new HashSet<Document>();

        public DateTime Date { get; set; }
    }
}