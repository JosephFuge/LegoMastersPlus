using LegoMastersPlus.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LegoMastersPlus.Components
{
    public class ThirdPartyAuthViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(LoginViewModel loginInfo, bool isLogin)
        {
            return View(loginInfo);
            //return View(isLogin ? "Login" : "Register", loginInfo);
        }
    }
}
