using System.ComponentModel.DataAnnotations;
using LegoMastersPlus.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using LegoMastersPlus.Models; 
namespace LegoMastersPlus.Pages;

public class CheckoutModel : PageModel
{
    private ILegoRepository _repo;
    public Cart Cart { get; set; }

    public CheckoutModel(ILegoRepository temp, Cart cartservice)
    {
        _repo = temp;
        Cart = cartservice;
    }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Shipping address is required")]
    public string ShippingAddress { get; set; }

    // Hidden fields not needing validation or user input
    public string Date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
    public string DayOfWeek { get; set; } = DateTime.Now.DayOfWeek.ToString();
    public int Time { get; set; } = DateTime.Now.Hour;
    public string EntryMode { get; set; } = "Online"; // Assuming this is a constant value
    public decimal Amount { get; set; } // This should be set from the cart's total
    public string TypeOfTransaction { get; set; } = "Sale"; // Assuming this might be a constant or selectable value

    [Required(ErrorMessage = "Country of transaction is required")]
    public string CountryOfTransaction { get; set; }

    [Required(ErrorMessage = "Bank is required")]
    public string Bank { get; set; }

    [Required(ErrorMessage = "Type of card is required")]
    public string TypeOfCard { get; set; }

    public void OnGet()
    {
        // Initialize properties if needed
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page(); // Return with errors
        }

        if (Cart.Lines.Count == 0)
        {
            ModelState.AddModelError("", "Your cart is empty!");
            return Page();
        }

        Amount = Cart.CalculateTotal(); // Set the total amount from the cart

        Order order = new Order
        {
            // Note: Customer ID needs to be set based on the logged-in user
            // Other properties set from the model
            shipping_address = ShippingAddress,
            date = Date,
            day_of_week = DayOfWeek,
            time = Time,
            entry_mode = EntryMode,
            amount = (int)Amount,
            type_of_transaction = TypeOfTransaction,
            country_of_transaction = CountryOfTransaction,
            bank = Bank,
            type_of_card = TypeOfCard,
            // Additional properties like 'fraud' flag need logic to be set
        };

        _repo.SaveOrder(order);
        Cart.Clear();
        return RedirectToPage("OrderConfirmation", new { transaction_ID = order.transaction_ID });
    }
}