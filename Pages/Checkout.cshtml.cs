using System.ComponentModel.DataAnnotations;
using LegoMastersPlus.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using LegoMastersPlus.Models;
using LegoMastersPlus.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using System;
using Microsoft.AspNetCore.Identity;
namespace LegoMastersPlus.Pages;

public class CheckoutModel : PageModel
{
    private ILegoRepository _repo;

    public Cart Cart { get; set; }
    private readonly InferenceSession _session;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly SignInManager<IdentityUser> _signInManager;


    public CheckoutModel(ILegoRepository temp, Cart cartservice, IWebHostEnvironment webHostEnvironment, SignInManager<IdentityUser> signInManager)
    {
        _repo = temp;
        Cart = cartservice;
        _webHostEnvironment = webHostEnvironment;
        _signInManager = signInManager;

        var modelPath = Path.Combine(webHostEnvironment.WebRootPath, "models", "fraud_catch_model.onnx");
        _session = new InferenceSession(modelPath);
    }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Shipping address is required")]
    [BindProperty]
    public string ShippingAddress { get; set; }

    // Hidden fields not needing validation or user input
    public string Date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
    public string DayOfWeek { get; set; } = DateTime.Now.DayOfWeek.ToString();
    public int Time { get; set; } = DateTime.Now.Hour;
    public string EntryMode { get; set; } = "CVC"; // Assuming this is a constant value
    public decimal Amount { get; set; } // This should be set from the cart's total
    public string TypeOfTransaction { get; set; } = "Online"; // Assuming this might be a constant or selectable value

    [Required(AllowEmptyStrings=false, ErrorMessage = "Country of transaction is required")]
    [BindProperty]
    public string CountryOfTransaction { get; set; }

    [Required(ErrorMessage = "Bank is required")]
    [BindProperty]
    public string Bank { get; set; }

    [Required(ErrorMessage = "Type of card is required")]
    [BindProperty]
    public string TypeOfCard { get; set; }

    public void OnGet()
    {
        // Initialize properties if needed
    }

    public Dictionary<String, int> Dummy(int Time, int Amount, string DayOfWeek, string EntryMode, string TypeOfTransaction, string CountryOfTransaction, string Bank, string TypeOfCard)
    {
        // Dummy code day of the week
        Dictionary<string, int> dayDict = new Dictionary<string, int>()
        {
            { "Mon", 0 },
            { "Tue", 0 },
            { "Wed", 0 },
            { "Thu", 0 },
            { "Fri", 0 },
            { "Sat", 0 },
            { "Sun", 0 }
        };

        if (DayOfWeek != "Fri")
        {
            dayDict[DayOfWeek] = 1; // Set the selected day to 1, others remain 0
        }

        // Dummy code entry mode
        Dictionary<string, int> entryModeDict = new Dictionary<string, int>()
        {
            { "pin", 0 },
            { "tap", 0 },
            { "cvc", 0 }
        };

        if (EntryMode != "cvc")
        {
            entryModeDict[EntryMode] = 1;
        }

        // Dummy code transaction type
        Dictionary<string, int> transactionTypeDict = new Dictionary<string, int>()
        {
            { "pin", 0 },
            { "tap", 0 },
            { "online", 0 },
            { "pos", 0 },
            { "atm", 0 }
        };

        if (TypeOfTransaction != "atm")
        {
            transactionTypeDict[TypeOfTransaction] = 1; // Set the selected transaction type to 1, others remain 0
        }

        // Dummy code country
        Dictionary<string, int> countryDict = new Dictionary<string, int>()
        {
            { "china", 0 },
            { "india", 0 },
            { "russia", 0 },
            { "uk", 0 },
            { "usa", 0 }
        };

        if (CountryOfTransaction != "china")
        {
            countryDict[CountryOfTransaction] = 1; // Set the selected country to 1, others remain 0
        }

        // Dummy code bank
        Dictionary<string, int> bankDict = new Dictionary<string, int>()
        {
            { "barclay", 0 },
            { "hsbc", 0 },
            { "halifax", 0 },
            { "lloyd", 0 },
            { "metro", 0 },
            { "monzo", 0 },
            { "rbs", 0 }
        };
        if (Bank != "barclay")
        {
            bankDict[Bank] = 1; // Set the selected bank to 1, others remain 0
        }

        // Dummy code card type
        Dictionary<string, int> cardTypeDict = new Dictionary<string, int>()
        {
            { "mastercard", 0 },
            { "visa", 0 }
        };
        if (TypeOfCard != "mastercard")
        {
            cardTypeDict[TypeOfCard] = 1; // Set the selected card type to 1, others remain 0
        }

        var inputVariables = new Dictionary<string, int>()
                {
                    { "time_hour", Time },
                    { "amount", Amount },
                    { "Mon", dayDict["Mon"] },
                    { "Sat", dayDict["Sat"] },
                    { "Sun", dayDict["Sun"] },
                    { "Thu", dayDict["Thu"] },
                    { "Tue", dayDict["Tue"] },
                    { "Wed", dayDict["Wed"] },
                    { "Pin", transactionTypeDict["pin"] },
                    { "Tap", transactionTypeDict["tap"] },
                    { "Online", transactionTypeDict["online"] },
                    { "POS", transactionTypeDict["pos"] },
                    { "India", countryDict["india"] },
                    { "Russia", countryDict["russia"] },
                    { "USA", countryDict["usa"] },
                    { "UK", countryDict["uk"] },
                    { "HSBC", bankDict["hsbc"] },
                    { "Halifax", bankDict["halifax"] },
                    { "Lloyds", bankDict["lloyd"] },
                    { "Metro", bankDict["metro"] },
                    { "Monzo", bankDict["monzo"] },
                    { "RBS", bankDict["rbs"] },
                    { "Visa", cardTypeDict["visa"] }
                };

        return inputVariables;
    }

    bool Predict(int Time, int Amount, string DayOfWeek, string EntryMode, string TypeOfTransaction, string CountryOfTransaction, string Bank, string TypeOfCard)
    //public IActionResult Predict(Dictionary<string, int> inputVariables)
    {
        //Bring in the dummy-coded data to be predicted
        var inputVariables = Dummy(Time, Amount, DayOfWeek, EntryMode, TypeOfTransaction, CountryOfTransaction, Bank, TypeOfCard);

        //Change the fraud prediction (boolean 0 or 1) into "not fraud" or "fraud"
        var fraud_dict = new Dictionary<int, string>()
            {
                { 0, "Not fraudulent"},
                { 1, "Possibly fradulent, please review"}
            };
        var input = new List<float>();
        foreach (var kvp in inputVariables)
        {
            input.Add(kvp.Value);
        }
        var inputTensor = new DenseTensor<float>(input.ToArray(), new[] { 1, input.Count });

        var inputs = new List<NamedOnnxValue>
            { NamedOnnxValue.CreateFromTensor("float_input", inputTensor)};

        using (var results = _session.Run(inputs)) //Make the predicton from the Order input
        {
            var prediction = results.FirstOrDefault(item => item.Name == "output_label")?.AsTensor<long>().ToArray();
            if (prediction != null && prediction.Length > 0)
            {
                //Get the proper fraud text
                //var fraudCheck = fraud_dict.GetValueOrDefault((int)prediction[0], "Unknown");
                //ViewBag.Prediction = fraudCheck;
                var isFraudulent = prediction[0] == 1;
                return isFraudulent;
            }
        }
        return false; //Return view of dummy data so I can check if it works
    }

    public async Task<IActionResult> OnPost()
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

        var isFraudulent = Predict(Time, (int)Amount, DayOfWeek, EntryMode, TypeOfTransaction, CountryOfTransaction, Bank, TypeOfCard);

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
            fraud = isFraudulent,
            // Additional properties like 'fraud' flag need logic to be set
        };

        var userClaim = HttpContext.User;

        if (userClaim != null)
        {
            var user = await _signInManager.UserManager.GetUserAsync(userClaim);
            if (user != null)
            {
                var customer = _repo.Customers.FirstOrDefault(c => c.IdentityID == user.Id);

                if (customer != null)
                {
                    order.customer_ID = customer.customer_ID; 
                    _repo.SaveOrder(order);
                    Cart.Clear();
                    return RedirectToPage("OrderConfirmation", new { value = isFraudulent });
                }
            }
        }
        return Page();
    }
}