using LegoMastersPlus.Controllers;
using LegoMastersPlus.Data;
using LegoMastersPlus.Models;
using LegoMastersPlus.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/*
 * Brickwell
 * Section 4 Group 4
 * Authors: Joseph Fuge, Cameron Klepacz, Ezekiel Goodman, Hannah Cameron
 */

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
var connectionString = config.GetConnectionString("LegoMasterConnection") ?? throw new InvalidOperationException("Connection string 'LegoMasterConnection' not found.");
builder.Services.AddDbContext<LegoMastersDbContext>(options =>
    options.UseSqlServer(connectionString));

// Dependency Injection for Repository pattern
builder.Services.AddScoped<ILegoRepository, EFLegoRepository>();

// Add and authorize razor pages
builder.Services.AddRazorPages(options =>
    {
        options.Conventions.AuthorizePage("/Checkout");
        options.Conventions.AuthorizePage("/OrderConfirmation");
    }
); 

// Shopping cart setup
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Set up Microsoft Identity and add restrictions to passwords
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 12;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredUniqueChars = 2;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
})
    .AddEntityFrameworkStores<LegoMastersDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();

// Add HSTS
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(60);
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.IsEssential = true;
    options.LoginPath = "/Home/Login";
    options.AccessDeniedPath = "/Home/AccessDenied";
    options.LogoutPath = "/Home/Logout";
    options.SlidingExpiration = true;
});
builder.Services.AddControllersWithViews();

// Add Third Party Google Auth
builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = config["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = config["Authentication:Google:ClientSecret"];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Add Customer and Admin roles if they don't exist
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<LegoMastersDbContext>();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Create roles if they don't exist
        if (!roleManager.RoleExistsAsync("Customer").Result)
        {
            roleManager.CreateAsync(new IdentityRole("Customer")).Wait();
        }
        if (!roleManager.RoleExistsAsync("Admin").Result)
        {
            roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
        }
    }
    catch (Exception ex)
    {
        // Log or handle the initialization error
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}


app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; img-src 'self' https://m.media-amazon.com https://www.brickeconomy.com https://www.lego.com https://images.brickset.com; script-src 'self' 'nonce-91556876980036' ");
    // Add more directives as needed
    await next();
});

// Redirect HTTP connections to HTTPS
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Improving URLs
app.MapControllerRoute("paginationOrders", "/Orders/{pageNum}", new { Controller = "Admin", action = "ReviewOrders", pageNum = 1 });
app.MapControllerRoute("paginationAdminProducts", "/Products/{pageNum}", new { Controller = "Admin", action = "Products", pageNum = 1 });
// app.MapControllerRoute("paginationAdminUsers", "/Users/{userPageNum}/{customerPageNum}", new { Controller = "Admin", action = "Users", userPageNum = 1, customerPageNum = 1 });
app.MapDefaultControllerRoute();

app.MapRazorPages();

app.Run();
