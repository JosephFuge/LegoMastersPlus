using Microsoft.AspNetCore.Identity;

namespace LegoMastersPlus.Models.ViewModels
{
    public class UserCustomerViewModel
    {
        public IdentityUser? IdentityUser { get; set; }
        public Customer? Customer { get; set; }
    }
}
