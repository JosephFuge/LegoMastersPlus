using LegoMastersPlus.Data;
using LegoMastersPlus.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LegoMastersPlus.Pages;

public class OrderConfirmation : PageModel
{
    private readonly ILegoRepository _repo;
    public Order Order { get; set; }
    public OrderConfirmation(ILegoRepository temp)
    {
        temp = _repo;
    }

    public int Transaction_ID { get; set; }
    public bool IsFraudulent { get; set; } // Assuming you want to use this flag in your view

    public void OnGet(bool value)
    {
            // Assuming you have a 'fraud' property in your Order class
            IsFraudulent = value;
    }
}