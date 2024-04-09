using LegoMastersPlus.Data;
using LegoMastersPlus.Models;
using LegoMastersPlus.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegoMastersPlus.Controllers
{
    // Ensure only Admins can access the pages and actions within AdminController
    [Authorize(Roles="Admin")]
    public class AdminController : Controller
    {
        private readonly ILegoRepository _legoRepo;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminController(ILegoRepository legoRepo, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _legoRepo = legoRepo;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Products(int pageNum) {
            const int pageSize = 10;

            // Set pageNum to 1 if it is 0 (as can happen for the default Products page request)
            pageNum = pageNum == 0 ? 1 : pageNum;

            // Get the correct list of products based on page size and page number
            var productList = _legoRepo.Products.Skip((pageNum - 1) * pageSize).Take(pageSize);

            // Gather paging info and product list into a ViewModel
            var productCount = _legoRepo.Products.Count();
            PaginationInfo pagingInfo = new PaginationInfo(productCount, pageSize, pageNum);
            var productPagingModel = new ProductsListViewModel(productList, pagingInfo);

            return View(productPagingModel);
        }

        public async Task<IActionResult> Users(int pageNum)
        {
            const int pageSize = 10;

            // Set pageNum to 1 if it is 0 (as can happen for the default Products page request)
            pageNum = pageNum == 0 ? 1 : pageNum;

            Dictionary<IdentityUser, IList<string>> allUserRoles = new Dictionary<IdentityUser, IList<string>>();

            //var customers = _legoRepo.Customers.Skip((pageNum - 1) * pageSize).Take(pageSize);

            // Get the correct list of users based on page size and page number
            var allUsers = await _userManager.Users.ToListAsync();
            var users = allUsers.Skip((pageNum - 1) * pageSize).Take(pageSize);

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles != null)
                {
                    allUserRoles.Add(user, userRoles);
                }
            }

            // Gather paging info and user list into a ViewModel
            var userCount = allUsers.Count();
            PaginationInfo pagingInfo = new PaginationInfo(userCount, pageSize, pageNum);
            var usersPagingModel = new UsersListViewModel(allUserRoles, pagingInfo);

            return View(usersPagingModel);
        }
    }
}
