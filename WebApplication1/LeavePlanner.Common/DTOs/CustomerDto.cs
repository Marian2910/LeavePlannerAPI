using System.ComponentModel.DataAnnotations;

namespace Common.DTOs
{
    public class CustomerDto
    {
        [Required(ErrorMessage = "The name of the customer is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Customer email is required!")]
        [EmailAddress(ErrorMessage = "Email not valid!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "You must provide a phone number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "The country of the customer is required")]
        public string Country { get; set; }
        [Required(ErrorMessage = "The city of the customer is required")]
        public string City { get; set; }
        [Required(ErrorMessage = "The postal code of the customer is required")]
        public string PostalCode { get; set; }
        [Required(ErrorMessage = "The street of the customer is required")]
        public string Street { get; set; }
        public string Number { get; set; }
        [Required(ErrorMessage = "The billing type is required")]
        public string BillingType { get; set; }
        [Range(0, 100, ErrorMessage = "TVA must be between 0 and 100.")]
        public int Tva { get; set; }
        public string Addition { get; set; }
        [Required(ErrorMessage = "The date is required")]
        public DateTime Date { get; set; }
    }
}
