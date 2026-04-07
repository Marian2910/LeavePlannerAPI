using System.ComponentModel.DataAnnotations;

namespace LeavePlanner.Domain.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Customer name is required!")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Customer email is required!")]
        [EmailAddress(ErrorMessage = "Email not valid!")]
        public required string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "The customer's country is required.")]
        public required string Country { get; set; }

        [Required(ErrorMessage = "The customer's city is required.")]
        public required string City { get; set; }

        [Required(ErrorMessage = "The postal code is required.")]
        public required string PostalCode { get; set; }

        [Required(ErrorMessage = "The street is required.")]
        public required string Street { get; set; }

        public string Number { get; set; } = string.Empty;

        public bool Status { get; set; } = true;

        [Required(ErrorMessage = "The billing type is required.")]
        public required string BillingType { get; set; }

        [Range(0, 100, ErrorMessage = "TVA must be between 0 and 100.")]
        public int Tva { get; set; }

        public string Addition { get; set; } = string.Empty;

        public ICollection<Document>? Documents { get; set; }

        [Required(ErrorMessage = "The date is required.")]
        public required DateTime Date { get; set; }
    }
}
