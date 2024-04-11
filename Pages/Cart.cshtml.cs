using LegoMastersPlus.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LegoMastersPlus.Infrastructure;
using LegoMastersPlus.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LegoMastersPlus.Pages;

public class CartModel : PageModel
{
    private ILegoRepository _repo;
    public Cart Cart { get; set; }

    
    public CartModel(ILegoRepository temp, Cart cartservice)
    {
        _repo = temp;
        Cart = cartservice;
    }
    
    public string ReturnUrl { get; set; }

    public void OnGet(string returnUrl)
    {
        ReturnUrl = returnUrl ?? "/";
        
    }

    public IActionResult OnPost(int product_ID, string returnUrl)
    {
        Product prod = _repo.Products
            .FirstOrDefault(x => x.product_ID == product_ID);
    
        if (prod != null)
        {
            Cart.AddItem(prod, 1);
        }
    
        return RedirectToPage(new { returnUrl = returnUrl });
    }
    
    public IActionResult OnPostRemove(int product_ID, string returnUrl)
    {
        
        var line = Cart.Lines.FirstOrDefault(x => x.Product.product_ID == product_ID);
        if (line != null)
        {
            Cart.RemoveLine(line.Product);
        }

        return RedirectToPage(new { returnUrl = returnUrl });
    }
    
    
    
}