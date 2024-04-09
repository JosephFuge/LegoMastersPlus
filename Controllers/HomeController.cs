using Humanizer;
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

namespace LegoMastersPlus.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILegoRepository _legoRepo;

        public HomeController(ILogger<HomeController> logger, SignInManager<IdentityUser> tempSignIn, ILegoRepository tempLegoRepo)
        {
            _logger = logger;
            _signInManager = tempSignIn;
            _legoRepo = tempLegoRepo;
        }

        public IActionResult Index()
        {
            return View();
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
            var result = await _signInManager.PasswordSignInAsync(loginRequest.Email, loginRequest.Password, loginRequest.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
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

        //public IActionResult CreateAccountInfo(string returnUrl = null, string remoteError = null)
        //{


        //}
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
