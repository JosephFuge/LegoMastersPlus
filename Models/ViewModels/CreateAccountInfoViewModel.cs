using System.ComponentModel.DataAnnotations;

namespace LegoMastersPlus.Models.ViewModels
{
    public class CreateAccountInfoViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Gender is required.")]
        public string gender { get; set; }
        // Age will be assumed by birthdate

        [Required(AllowEmptyStrings = false, ErrorMessage = "Birthdate is required.")]
        public DateTime birth_date { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Country of Residence is required.")]
        public string country_of_residence { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required.")]
        public string first_name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required.")]
        public string last_name { get; set; }

        public string ReturnUrl { get; set; } = "/Home/Index";
    }
}
