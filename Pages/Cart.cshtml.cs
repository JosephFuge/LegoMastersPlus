using Humanizer;
using LegoMastersPlus.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LegoMastersPlus.Infrastructure;
using LegoMastersPlus.Models;
using Microsoft.CodeAnalysis;

namespace LegoMastersPlus.Pages;

public class CartModel : PageModel
{
    private ILegoRepository _repo;
    public Cart Cart { get; set; }

    
    public CartModel(ILegoRepository temp, Cart cartservice)
    {
        _repo = temp;
        Cart - cartservice;
    }
    
    public string ReturnUrl { get; set; }

    public void OnGet(string returnUrl)
    {
        ReturnUrl = returnUrl ?? "/";
        
    }

    public IActionResult OnPost(int projectId, string returnUrl)
    {
        Project proj = _repo.Products
            .FirstOrDefault(x => x.product_ID == projectId);

        if (proj != null)
        {
            Cart.AddItem(proj, 1);
        }

        return RedirectToPage(new { returnUrl = returnUrl });
    }

    public IActionResult OnPostRemove(int projectId, string returnUrl)
    {
        Cart.RemoveLine(Cart.Lines.First(x => x.Project.ProjectId).Project);
        return RedirectToPage(new { returnUrl = returnUrl });
    }
    
    
}