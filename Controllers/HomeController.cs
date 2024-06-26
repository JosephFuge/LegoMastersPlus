using LegoMastersPlus.Data;
using LegoMastersPlus.Models;
using LegoMastersPlus.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Numerics.Tensors;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.NetworkInformation;
using System.Linq;
using Microsoft.EntityFrameworkCore;

/*
 * Brickwell
 * Section 4 Group 4
 * Authors: Joseph Fuge, Cameron Klepacz, Ezekiel Goodman, Hannah Cameron
 */

namespace LegoMastersPlus.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILegoRepository _legoRepo;
        private readonly InferenceSession _session;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, SignInManager<IdentityUser> tempSignIn, ILegoRepository tempLegoRepo, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _signInManager = tempSignIn;
            _legoRepo = tempLegoRepo;
            _webHostEnvironment = webHostEnvironment;

            var modelPath = Path.Combine(webHostEnvironment.WebRootPath, "models", "fraud_catch_model.onnx");
            _session = new InferenceSession(modelPath);
        }

        
        public async Task<IActionResult> Index()
        {
            var products = getTopProducts();

            var userClaim = HttpContext.User;

            // If user is signed in, has a customer row in the customer table and has a pre-predicted recommendation for them in the database, show it
            // Otherwise, show top-rated products
            if (userClaim != null)
            {
                var user = await _signInManager.UserManager.GetUserAsync(userClaim);
                if (user != null)
                {
                    var customer = _legoRepo.Customers.FirstOrDefault(c => c.IdentityID == user.Id);

                    if (customer != null)
                    {
                        var customerRecs = _legoRepo.ProductUserRecommendations(customer.customer_ID).FirstOrDefault();
                        if (customerRecs != null)
                        {
                            products = [
                                customerRecs.Product_1,
                                customerRecs.Product_2,
                                customerRecs.Product_3,
                                customerRecs.Product_4,
                                customerRecs.Product_5,
                                customerRecs.Product_6,
                                customerRecs.Product_7,
                                customerRecs.Product_8,
                                customerRecs.Product_9,
                                customerRecs.Product_10
                                ];
                        }
                    }
                }
            }

            return View(products);
        }

        private List<Product> getTopProducts()
        {
            // 1. Get top-purchased product Ids
            var topProductIds = _legoRepo.LineItems
               .GroupBy(li => li.product_ID)
               .Select(group => new { ProductId = group.Key, PurchaseCount = group.Count() })
               .OrderByDescending(x => x.PurchaseCount)
               .Take(5) // Taking only the top 5
               .Select(x => x.ProductId)
               .ToList();

            // 2. Join with Product details
            var topProducts = _legoRepo.Products
                .Where(p => topProductIds.Contains(p.product_ID))
                .ToList();

            return topProducts;
        }

        public IActionResult AccessDenied(string ReturnUrl)
        {
            return View();
        }

        public IActionResult ProductDetails(int product_ID)
        {
            Product? prod = _legoRepo.Products.FirstOrDefault(p => p.product_ID == product_ID);
            
            if (prod == null)
            {
                return RedirectToAction("Products");
            }

            var details = _legoRepo.ProductItemRecommendations(product_ID).FirstOrDefault();
            List<Product>? products = null;
            if (details == null)
            {
                var productIds = _legoRepo.LineItems
                   .Where(li => li.product_ID != product_ID)
                   .GroupBy(li => li.product_ID)
                   .Select(group => new { ProductId = group.Key, PurchaseCount = group.Count() })
                   .OrderByDescending(x => x.PurchaseCount)
                   .Take(5) // Taking only the top 5
                   .Select(x => x.ProductId)
                   .ToList();
                products = _legoRepo.Products
                    .Where(p => productIds.Contains(p.product_ID))
                    .ToList();
            }

            ProductDetailsViewModel prodDetails = new ProductDetailsViewModel
            {
                product_ID = prod.product_ID,
                RecProduct = prod,
                Recommendation = details,
                StaticRecommendations = products,
            };
            return View(prodDetails);
        }

        public IActionResult Privacy()
        {
            return View();
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
        public async Task<IActionResult> CustomerRegister()
        {
            return View(new CustomerRegisterViewModel
            {
                SignInAfter = true,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> CustomerRegister(CustomerRegisterViewModel customerRegister)
        {
            if (!_isCookieConsentAccepted())
            {
                return View(customerRegister);
            }

            if (ModelState.IsValid)
            {
                var newUser = new IdentityUser();

                newUser.Email = customerRegister.Email;
                newUser.UserName = customerRegister.Email;

                IdentityResult registerResult = await _signInManager.UserManager.CreateAsync(newUser, customerRegister.Password);
                if (registerResult.Succeeded)
                {
                    await _signInManager.UserManager.AddToRoleAsync(newUser, "Customer");

                    var user = await _signInManager.UserManager.FindByEmailAsync(customerRegister.Email);

                    var newCustomer = new Customer
                    {
                        IdentityID = user?.Id,
                        first_name = customerRegister.first_name,
                        gender = customerRegister.gender,
                        last_name = customerRegister.last_name,
                        birth_date = DateOnly.FromDateTime(customerRegister.birth_date),
                        country_of_residence = customerRegister.country_of_residence,
                        age = CalculateAge(customerRegister.birth_date, DateTime.Now)
                    };

                    _legoRepo.AddCustomer(newCustomer);

                    _logger.LogInformation("Customer created with name " + customerRegister.first_name + " " + customerRegister.last_name);


                    // If they should sign in (defaults to yes), go to the home page
                    if (customerRegister.SignInAfter)
                    {
                        await _signInManager.SignInAsync(newUser, isPersistent: false);
                        return RedirectToPage("/Account/Manage/EnableAuthenticator", new { Area = "Identity", RememberMe = false, returnUrl = "/" });
                    } else
                    {
                        // Otherwise, check if they are an admin and if so, take them back to the Users page
                        var userClaim = HttpContext.User;
                        if (userClaim != null)
                        {
                            var tempUser = await _signInManager.UserManager.GetUserAsync(userClaim);
                            if (tempUser != null && !(await _signInManager.UserManager.IsInRoleAsync(tempUser, "Admin")))
                            {
                                return RedirectToAction("Users", "Admin");
                            } else
                            {
                                return RedirectToPage("/Account/Manage/EnableAuthenticator", new { Area = "Identity", RememberMe = false, returnUrl = "/" });
                            }
                        }
                        return RedirectToPage("/Account/Manage/EnableAuthenticator", new { Area = "Identity", RememberMe = false, returnUrl = "/" });
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

                    return View(customerRegister);
                }
            }
            else
            {
                return View(customerRegister);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            // If there's already a logged in user, redirect them to home page
            var curUserClaim = HttpContext.User;
           
            if (curUserClaim != null)
            {
                IdentityUser? curUser = await _signInManager.UserManager.GetUserAsync(curUserClaim);

                if (curUser != null)
                {
                    return RedirectToAction("Index", "Home");
                } else {
                    return View(new LoginViewModel
                    {
                        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
                    });
                }
            } else
            {
                return View(new LoginViewModel
                    {
                        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
                    });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginRequest, string returnUrl = null)
        {
            if (!_isCookieConsentAccepted())
            {
                return View(loginRequest);
            }

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginRequest.Email, loginRequest.Password, loginRequest.RememberMe, lockoutOnFailure: false);
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("/Account/LoginWith2f", new { Area = "Identity", RememberMe = loginRequest.RememberMe, returnUrl = "/" });
                }
                if (result.Succeeded)
                {
                    
                    // var user = await _userManager.FindByEmailAsync(loginRequest.Email);
                    // if user != null && await _userManager.GetTwoFactorEnabledAsync(user)
                    
                    if (loginRequest.Email == "haydencowart@faketest.com" || loginRequest.Email == "aurorabrickwell@legomasters.com")
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // Redirect to the LoginWith2fa page
                        // Redirect to the LoginWith2fa Razor Page
                        return RedirectToPage("/Account/Manage/EnableAuthenticator", new { Area = "Identity", RememberMe = loginRequest.RememberMe, returnUrl = "/" });
                    }
                    
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login information.");
                        return View(loginRequest);
                    }
                } else
            {
                if (loginRequest.Email == "haydencowart@faketest.com" || loginRequest.Email == "aurorabrickwell@legomasters.com")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    // Redirect to the LoginWith2fa page
                    // Redirect to the LoginWith2fa Razor Page
                    return RedirectToPage("/Account/LoginWith2f", new { Area = "Identity", RememberMe = loginRequest.RememberMe, returnUrl = "/" });
                }
            }
        }

        [HttpGet]
        public IActionResult ExternalLogin()
        {
            // Shouldn't be able to navigate here; just give them the login page
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider)
        {
            string redirectUrl = "/Home/CreateAccountInfo";
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> CreateAccountInfo(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= "/Home/Index";

            if (remoteError != null)
            {
                return LocalRedirect(returnUrl);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info!.Principal?.Identity?.Name ?? "User", info.LoginProvider);
                    return LocalRedirect(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    return LocalRedirect("/Home/Index");
                }
                else
                {
                    // If the user does not have an account, then ask the user to create an account.
                    CreateAccountInfoViewModel createAccountInfo = new CreateAccountInfoViewModel
                    {
                        ReturnUrl = returnUrl
                    };


                    if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                    {
                        createAccountInfo.Email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    }
                    return View(createAccountInfo);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccountInfo(CreateAccountInfoViewModel newCustomerInfo)
        {
            if (!_isCookieConsentAccepted())
            {
                return View(newCustomerInfo);
            }

            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                // TODO: add error if it went wrong
                return LocalRedirect("Index");
            }

            if (ModelState.IsValid)
            {
                var user = new IdentityUser {
                    Email = newCustomerInfo.Email,
                    UserName = newCustomerInfo.Email
                };

                //await SetUserNameAsync(user, newCustomerInfo.Email, CancellationToken.None);
                //await SetEmailAsync(user, newCustomerInfo.Email, CancellationToken.None);

                var result = await _signInManager.UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _signInManager.UserManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        await _signInManager.UserManager.AddToRoleAsync(user, "Customer");

                        var newCustomer = new Customer
                        {
                            first_name = newCustomerInfo.first_name,
                            gender = newCustomerInfo.gender,
                            last_name = newCustomerInfo.last_name,
                            birth_date = DateOnly.FromDateTime(newCustomerInfo.birth_date),
                            country_of_residence = newCustomerInfo.country_of_residence,
                            age = CalculateAge(newCustomerInfo.birth_date, DateTime.Now)
                        };

                        _legoRepo.AddCustomer(newCustomer);

                        _logger.LogInformation("Customer created with name " + newCustomerInfo.first_name + " " + newCustomerInfo.last_name);

                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                        return LocalRedirect(newCustomerInfo.ReturnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return LocalRedirect(newCustomerInfo.ReturnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Consent()
        {
            Response.Cookies.Append("CookieConsent", "true", new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private bool _isCookieConsentAccepted()
        {
            if (!Request.Cookies.ContainsKey("CookieConsent"))
            {
                ViewBag.ShowCookieConsentButton = true;
                ModelState.AddModelError("", "You must accept cookies before logging in.");
                return false;
            } else
            {
                ViewBag.ShowCookieConsentButton = false;
                //if (ModelState.ContainsKey("Cookies"))
                //{
                //    ModelState.Remove("Cookies");
                //}
                return true;
            }
        }

        public IActionResult Products(ProductsListViewModel plvm)
        {
            plvm.Categories = _legoRepo.Categories.ToList();

            if (/*plvm.Categories != null || */plvm.PrimaryColor != null || plvm.SecondaryColor != null || (plvm.SelectedCategories != null && plvm.Categories != null && plvm.SelectedCategories.Count() < plvm.Categories.Count()))
            {
                plvm.NoFilters = false;
            }
            int defaultPageSize = 10;

            // Fetch the list of possible colors based on the colors that products already have in the database
            plvm.PrimaryColors = _legoRepo.PrimaryColors.ToList();
            plvm.SecondaryColors = _legoRepo.SecondaryColors.ToList();

            if (plvm.SelectedCategories == null)
            {
                plvm.SelectedCategories = _legoRepo.Categories.Select(x => x.CategoryId).ToList();
            }

            // Filter by color in the query
            var filteredProducts = _legoRepo.Products
                .Where(x => plvm.NoFilters || ((plvm.PrimaryColor == null || x.primary_color == plvm.PrimaryColor) &&
                            (plvm.SecondaryColor == null || x.secondary_color == plvm.SecondaryColor))).ToList();
            
            // Now filter by category
            if (!plvm.NoFilters)
            {
                filteredProducts = filteredProducts.Where(prod => prod.ProductCategories.Select(pc => pc.Category.CategoryId).Intersect(plvm.SelectedCategories).Any()).ToList();
            }
            
            if (!plvm.Products.Any())
            {
                plvm.PageSize = plvm.PageSize == 0 ? 10 : plvm.PageSize;
                // Set pageNum to 1 if it is 0 (as can happen for the default Products page request)
                plvm.PageNum = plvm.PageNum == 0 ? 1 : plvm.PageNum;

                // Get the correct list of products based on page size and page number
                var productList = filteredProducts.Skip(((plvm.PageNum ?? 1) - 1) * (plvm.PageSize ?? defaultPageSize)).Take(plvm.PageSize ?? defaultPageSize);

                // Gather paging info and product list into a ViewModel
                var productCount = filteredProducts.Count();
                PaginationInfo pagingInfo = new PaginationInfo(productCount, plvm.PageSize ?? defaultPageSize, plvm.PageNum ?? 1);
                plvm.Products = productList.ToList();
                plvm.PaginationInfo = pagingInfo;
            } else
            {
                var allProducts = _legoRepo.Products;
                PaginationInfo pagingInfo = new PaginationInfo(allProducts.Count(), defaultPageSize, 1);
                var allCategories = _legoRepo.Categories;
                plvm = new ProductsListViewModel(allProducts.ToList(), pagingInfo, null, null, allCategories.ToList(), null, null);
            }

            return View(plvm);
        }

        [HttpGet]
        public IActionResult AboutUs()
        {
            return View();
        }
    }
}