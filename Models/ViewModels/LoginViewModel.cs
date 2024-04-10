using Microsoft.AspNetCore.Authentication;
using System;
using System.ComponentModel.DataAnnotations;

namespace LegoMastersPlus.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;

        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;

        public IList<AuthenticationScheme>? ExternalLogins { get; set; }

        //public string ReturnUrl { get; set; } = Url.Content("~/");
    }
}
