using Microsoft.AspNetCore.Mvc;
using LegoMastersPlus.Models;
using LegoMastersPlus.Data;

namespace LegoMastersPlus.Components
{
    public class ProductCategorysViewComponent : ViewComponent
    {
        private ILegoRepository _legoRepo;

        // Constructor
        public ProductCategorysViewComponent(ILegoRepository temp)
        {
            _legoRepo = temp;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedProductCategory = RouteData?.Values["productCategory"];

            var productCategorys = _legoRepo.Products
                .Select(x => x.category)
                .Distinct()
                .OrderBy(x => x);

            return View(productCategorys);
        }
    }
}
