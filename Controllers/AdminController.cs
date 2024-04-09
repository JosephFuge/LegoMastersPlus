using LegoMastersPlus.Data;
using LegoMastersPlus.Models;
using LegoMastersPlus.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegoMastersPlus.Controllers
{
    [Authorize(Roles="Admin")]
    public class AdminController : Controller
    {
        private readonly ILegoRepository _legoRepo;
        public AdminController(ILegoRepository legoRepo)
        {
            _legoRepo = legoRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Products(int pageNum) {
            const int pageSize = 10;

            // Set pageNum to 1 if it is 0 (as can happen for the default Products page request)
            pageNum = pageNum == 0 ? 1 : pageNum;

            // Get the correct list of books based on page size and page number
            var bookList = _legoRepo.Products.Skip((pageNum - 1) * pageSize).Take(pageSize);

            // Gather paging info and book list into a ViewModel
            var bookCount = _legoRepo.Products.Count();
            ProductPaginationInfo pagingInfo = new ProductPaginationInfo(bookCount, pageSize, pageNum);
            var bookPagingModel = new ProductsListViewModel(bookList, pagingInfo);

            return View(bookPagingModel);
        }
    }
}
