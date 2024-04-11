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
        private readonly ILogger<HomeController> _logger;
        private readonly ILegoRepository _legoRepo;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminController(ILogger<HomeController> logger, ILegoRepository legoRepo, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _legoRepo = legoRepo;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
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
        public  IActionResult DeleteProduct(int product_ID)
        {
            // Logic to delete the product from the repository
            var product = _legoRepo.Products.FirstOrDefault(p => p.product_ID == product_ID);
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
        public IActionResult EditProduct(int product_ID)
        {
            ViewBag.Categories = _legoRepo.Categories.Distinct().ToList();
            var editProduct = _legoRepo.Products.Single(prod => prod.product_ID == product_ID);
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
            var allUsers = await _signInManager.UserManager.Users.ToListAsync();
            var users = allUsers.Skip((pageNum - 1) * pageSize).Take(pageSize);

            foreach (var user in users)
            {
                var userRoles = await _signInManager.UserManager.GetRolesAsync(user);
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

        private double CalculateAge(DateTime birthdate, DateTime currentDate)
        {
            // Calculate the full years
            int years = currentDate.Year - birthdate.Year;
            if (currentDate.Month < birthdate.Month || (currentDate.Month == birthdate.Month && currentDate.Day < birthdate.Day))
            {
                years--;
            }

            // Calculate the number of days in the last year to get the decimal part
            DateTime lastBirthday = birthdate.AddYears(years);
            DateTime nextBirthday = lastBirthday.AddYears(1);
            double daysInYear = (nextBirthday - lastBirthday).TotalDays;
            double daysAfterLastBirthday = (currentDate - lastBirthday).TotalDays;

            // Calculate the decimal part of the age
            double decimalAge = daysAfterLastBirthday / daysInYear;

            return years + decimalAge;
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            return View("~/Views/Home/CustomerRegister.cshtml", new CustomerRegisterViewModel
            {
                SignInAfter = false
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(CustomerRegisterViewModel customerRegister)
        {
            if (!_isCookieConsentAccepted())
            {
                return View("~/Views/Home/CustomerRegister.cshtml", customerRegister);
            }

            if (ModelState.IsValid)
            {
                var newUser = new IdentityUser();

                newUser.Email = customerRegister.Email;
                newUser.UserName = customerRegister.Email;

                // Create customer user account
                IdentityResult registerResult = await _signInManager.UserManager.CreateAsync(newUser, customerRegister.Password);
                if (registerResult.Succeeded)
                {
                    await _signInManager.UserManager.AddToRoleAsync(newUser, "Customer");

                    // Create customer model. Calculate age based off of time since birth date
                    var newCustomer = new Customer
                    {
                        first_name = customerRegister.first_name,
                        gender = customerRegister.gender,
                        last_name = customerRegister.last_name,
                        birth_date = DateOnly.FromDateTime(customerRegister.birth_date),
                        country_of_residence = customerRegister.country_of_residence,
                        age = CalculateAge(customerRegister.birth_date, DateTime.Now)
                    };

                    _legoRepo.AddCustomer(newCustomer);

                    _logger.LogInformation("Admin created a customer with name " + customerRegister.first_name + " " + customerRegister.last_name);


                    // If they should sign in (defaults to yes), go to the home page
                    if (customerRegister.SignInAfter)
                    {
                        await _signInManager.SignInAsync(newUser, isPersistent: false);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // Otherwise, check if they are an admin and if so, take them back to the Users page
                        var userClaim = HttpContext.User;
                        if (userClaim != null)
                        {
                            var user = await _signInManager.UserManager.GetUserAsync(userClaim);
                            if (await _signInManager.UserManager.IsInRoleAsync(user, "Admin"))
                            {
                                return RedirectToAction("Users", "Admin");
                            }
                            else
                            {
                                return RedirectToAction("Index");
                            }
                        }
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    if (registerResult.Errors.Any())
                    {
                        foreach (IdentityError err in registerResult.Errors)
                        {
                            if (err.Code.Contains("Password"))
                            {
                                ModelState.AddModelError("Password", err.Description);
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, err.Description);
                            }
                        }
                    }

                    return View("~/Views/Home/CustomerRegister.cshtml", customerRegister);
                }
            }
            else
            {
                return View("~/Views/Home/CustomerRegister.cshtml", customerRegister);
            }
        }

        private bool _isCookieConsentAccepted()
        {
            if (!Request.Cookies.ContainsKey("CookieConsent"))
            {
                ViewBag.ShowCookieConsentButton = true;
                ModelState.AddModelError("", "You must accept cookies before logging in.");
                return false;
            }
            else
            {
                ViewBag.ShowCookieConsentButton = false;
                //if (ModelState.ContainsKey("Cookies"))
                //{
                //    ModelState.Remove("Cookies");
                //}
                return true;
            }
        }
    }
}
