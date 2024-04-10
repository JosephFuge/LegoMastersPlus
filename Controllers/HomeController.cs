//using Humanizer;
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

namespace LegoMastersPlus.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILegoRepository _legoRepo;
        private readonly InferenceSession _session;

        public HomeController(ILogger<HomeController> logger, SignInManager<IdentityUser> tempSignIn, ILegoRepository tempLegoRepo)
        {
            _logger = logger;
            _signInManager = tempSignIn;
            _legoRepo = tempLegoRepo;

            // Initialize the InferenceSession
            try
            {
                _session = new InferenceSession("C:\\Users\\theul\\source\\repos\\LegoMastersPlus\\fraud_catch_model.onnx");
                _logger.LogInformation("ONNX model loaded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading the ONNX model: {ex.Message}");
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ProductDetails(int productId)
        {
            var details = _legoRepo.ProductItemRecommendations(productId).FirstOrDefault();
            return View(details);
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
        public IActionResult CustomerRegister()
        {
            return View();
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

                    _logger.LogInformation("Customer created with name " + customerRegister.first_name + " " + customerRegister.last_name);
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    return View("Index");
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
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginRequest)
        {
            if (!_isCookieConsentAccepted())
            {
                return View(loginRequest);
            }

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginRequest.Email, loginRequest.Password, loginRequest.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login information.");
                    return View(loginRequest);
                }
            } else
            {
                return View(loginRequest);
            }
        }

        [HttpGet]
        public IActionResult ExternalLogin()
        {
            return View("Login");
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

        //ONNX Prediction
        //Review for later; it only predicts 1 (not fradulent) for some reason
        [HttpPost]
        public IActionResult Predict(int time_hour, int amount, int Mon, int Sat, int Sun, int Thr, int Tue, int Wed, int Pin, int Tap, int Online, int POS, int India, int Russia, int USA, int UK, int HSBC, int Halifax, int Lloyds, int Metro, int Monzo, int RBS, int Visa)
        {
            //Change the fraud prediction (boolean 0 or 1) into "not fraud" or "fraud"
            var fraud_dict = new Dictionary<int, string>()
            {
                { 1, "Not fraudulent"},
                { 2, "Possibly fradulent, please review"}
            };
            try
            {
                var input = new List<float> { time_hour, amount, Mon, Sat, Sun, Thr, Tue, Wed, Pin, Tap, Online, POS, India, Russia, USA, UK, HSBC, Halifax, Lloyds, Metro, Monzo, RBS, Visa };
                var inputTensor = new DenseTensor<float>(input.ToArray(), new[] { 1, input.Count });

                var inputs = new List<NamedOnnxValue>
                { NamedOnnxValue.CreateFromTensor("float_input", inputTensor)};

                using (var results = _session.Run(inputs)) //Make the predicton from the Order input
                {
                    var prediction = results.FirstOrDefault(item => item.Name == "output_label")?.AsTensor<long>().ToArray();
                    if (prediction != null && prediction.Length > 0)
                    {
                        //Get the proper fraud text
                        var fraudCheck = fraud_dict.GetValueOrDefault((int)prediction[0], "Unknown");
                        ViewBag.Prediction = fraudCheck;
                    }
                    else
                    {
                        ViewBag.Prediction = "Error: Unable to make a prediction.";
                    }
                }
                _logger.LogInformation("Prediction executed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during prediction: {ex.Message}");
                ViewBag.Prediction = "Error during prediction.";
            }

            return View(); //Return view of dummy data so I can check if it works
        }

        //For the Predict view; delete when connceted to the database
        [HttpGet]
        public IActionResult Predict()
        {
            return View();
        }

        public IActionResult Products(int pageNum, int pageSize, string productPrimColor, string productSecColor, string productCategory)
        {

            var filteredProducts = _legoRepo.Products
                .Where(x => (productPrimColor == null || x.primary_color == productPrimColor) &&
                            (productSecColor == null || x.secondary_color == productSecColor) &&
                            (productCategory == null));

        

            
            pageSize = 12;
            // Set pageNum to 1 if it is 0 (as can happen for the default Products page request)
            pageNum = pageNum == 0 ? 1 : pageNum;

            // Get the correct list of products based on page size and page number
            var productList = _legoRepo.Products.Skip((pageNum - 1) * pageSize).Take(pageSize);

            // Gather paging info and product list into a ViewModel
            var productCount = _legoRepo.Products.Count();
            PaginationInfo pagingInfo = new PaginationInfo(productCount, pageSize, pageNum);
            var productPagingModel = new ProductsListViewModel(productList, pagingInfo);
            
            // var data = new ProductsListViewModel
            // {
            //     Products = filteredProducts
            //         .OrderBy(x => x.name)
            //         .Skip((pageNum - 1) * pageSize)
            //         .Take(pageSize),
            //     
            //     PaginationInfo = new ProductPaginationInfo
            //     {
            //
            //         CurrentPage = pageNum,
            //         ItemsPerPage = pageSize,
            //         TotalItems = filteredProducts.Count()
            //     }};

            return View(productPagingModel);
        }

        [HttpGet]
        public IActionResult AboutUs()
        {
            return View();
        }

    }
}