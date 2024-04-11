using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LegoMastersPlus.Pages;

public class OrderConfirmation : PageModel
{
    public int TransactionID { get; set; }

    public void OnGet(int transaction_ID)
    {
        TransactionID = transaction_ID;
        // Additional logic to retrieve and display order details based on TransactionID
    }
}