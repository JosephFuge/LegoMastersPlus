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

    public IActionResult OnPost(string action, int productId, string returnUrl)
    {
        switch (action)
        {
            case "add":
                // Logic to add an item
                Product prodToAdd = _repo.Products.FirstOrDefault(x => x.product_ID == productId);
                if (prodToAdd != null)
                {
                    Cart.AddItem(prodToAdd, 1);
                }
                break;
            case "remove":
                // Logic to remove an item
                Product prodToRemove = _repo.Products.FirstOrDefault(x => x.product_ID == productId);
                if (prodToRemove != null)
                {
                    Cart.RemoveLine(prodToRemove);
                }
                break;
        }

        return RedirectToPage(new { returnUrl = returnUrl });
    }

    // public IActionResult OnPost(int productId, string returnUrl)
    // {
    //     Product prod = _repo.Products
    //         .FirstOrDefault(x => x.product_ID == productId);
    //
    //     if (prod != null)
    //     {
    //         Cart.AddItem(prod, 1);
    //     }
    //
    //     return RedirectToPage(new { returnUrl = returnUrl });
    // }
    //
    public IActionResult OnPostRemove(int productId, string returnUrl)
    {
        
        // Cart.RemoveLine(Cart.Lines.First(x => x.Product.product_ID == productId).Product);
        Cart.RemoveLine(Cart.Lines.First(x => x.Product.product_ID == productId).Product);
       
        return RedirectToPage(new { returnUrl = returnUrl });
    }
    
    
    
}