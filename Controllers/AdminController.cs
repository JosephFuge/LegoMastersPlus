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
            var productPagingModel = new ProductsListViewModel(productList.ToList(), pagingInfo, null, null, Enumerable.Empty<Category>().ToList(), null, null);

            return View(productPagingModel);
        }
        
        [HttpPost]
        public  IActionResult DeleteProduct(int productId)
        {
            // Logic to delete the product from the repository
            var product = _legoRepo.Products.FirstOrDefault(p => p.product_ID == productId);
            if (product != null)
            {
                _legoRepo.DeleteProduct(product); 
                _legoRepo.SaveChanges(); 
            }

            return RedirectToAction("Products");
        }
        
        [HttpPost]
        public  IActionResult DeleteOrder(int transactionId)
        {
            // Logic to delete the product from the repository
            var order = _legoRepo.Orders.FirstOrDefault(o => o.transaction_ID == transactionId);
            if (order != null)
            {
                _legoRepo.DeleteOrder(order); 
                _legoRepo.SaveChanges(); 
            }

            return RedirectToAction("ReviewOrders");
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            ViewBag.Categories = _legoRepo.Categories.Distinct().ToList();
            return View("AddEditProduct", new Product());
        }
        
        [HttpPost]
        public IActionResult AddProduct(Product newProduct)
        {
            if (ModelState.IsValid)
            {
                _legoRepo.AddProduct(newProduct);
                return RedirectToAction("Products");
            } else
            {
                ViewBag.Categories = _legoRepo.Categories.Distinct().ToList();
                return View("AddEditProduct", newProduct);
            }
        }

        [HttpGet]
        public IActionResult EditProduct(int productId)
        {
            ViewBag.Categories = _legoRepo.Categories.Distinct().ToList();
            var editProduct = _legoRepo.Products.Single(prod => prod.product_ID == productId);
            return View("AddEditProduct", editProduct);
        }

        [HttpPost]
        public IActionResult EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _legoRepo.UpdateProduct(product);
                return RedirectToAction("Products");
            }
            else
            {
                ViewBag.Categories = _legoRepo.Categories.Distinct().ToList();
                return View("AddEditProduct", product);
            }
        }

        public async Task<IActionResult> Users(int pageNum)
        {
            const int pageSize = 10;

            // Set pageNum to 1 if it is 0 (as can happen for the default Users page request)
            pageNum = pageNum == 0 ? 1 : pageNum;

            Dictionary<IdentityUser, IList<string>> allUserRoles = new Dictionary<IdentityUser, IList<string>>();

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
        
        public IActionResult ReviewOrders(int pageNum)
        {
            const int pageSize = 100;

            // Set pageNum to 1 if it is 0 (as can happen for the default Products page request)
            pageNum = pageNum == 0 ? 1 : pageNum;

            // Get the correct list of products based on page size and page number
            var orderList = _legoRepo.Orders.Skip((pageNum - 1) * pageSize).Take(pageSize);

            // Gather paging info and product list into a ViewModel
            var orderCount = _legoRepo.Orders.Count();
            PaginationInfo pagingInfo = new PaginationInfo(orderCount, pageSize, pageNum);
            var orderPagingModel = new OrdersListViewModel(orderList, pagingInfo);   
            return View(orderPagingModel);
        }
    }
}
