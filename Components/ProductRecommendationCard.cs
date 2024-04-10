using LegoMastersPlus.Models;
using LegoMastersPlus.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LegoMastersPlus.Components
{
    public class ProductRecommendationCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(Product product)
        {
            return View(product);
        }
    }
}
