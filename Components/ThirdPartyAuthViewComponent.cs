using LegoMastersPlus.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace LegoMastersPlus.Components
{
    public class ThirdPartyAuthViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<AuthenticationScheme>? externalLoginInfo, bool isLogin)
        {
            return View(externalLoginInfo ?? new List<AuthenticationScheme>());
            //return View(isLogin ? "Login" : "Register", loginInfo);
        }
    }
}
