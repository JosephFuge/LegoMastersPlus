using Microsoft.AspNetCore.Identity;

namespace LegoMastersPlus.Models.ViewModels
{
    public class UsersListViewModel
    {
        public IDictionary<IdentityUser, IList<string>> UserRoles { get; set; }

        public PaginationInfo UserPaginationInfo { get; set; } = new PaginationInfo();

        public PaginationInfo CustomerPaginationInfo { get; set; } = new PaginationInfo();

        public List<Customer> Customers { get; set; }

        public UsersListViewModel(IDictionary<IdentityUser, IList<string>> tempUserRoles, PaginationInfo tempUserPaginationInfo, PaginationInfo tempCustomerPaginationInfo, List<Customer> customers)
        {
            this.UserRoles = tempUserRoles;
            this.UserPaginationInfo = tempUserPaginationInfo;
            this.CustomerPaginationInfo = tempCustomerPaginationInfo;
            this.Customers = customers;
        }
    }
}
