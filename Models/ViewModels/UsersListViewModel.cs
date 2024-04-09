using Microsoft.AspNetCore.Identity;

namespace LegoMastersPlus.Models.ViewModels
{
    public class UsersListViewModel
    {
        public IDictionary<IdentityUser, IList<string>> UserRoles { get; set; }

        public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();

        public UsersListViewModel(IDictionary<IdentityUser, IList<string>> tempUserRoles, PaginationInfo tempPaginationInfo)
        {
            this.UserRoles = tempUserRoles;
            this.PaginationInfo = tempPaginationInfo;
        }
    }
}
