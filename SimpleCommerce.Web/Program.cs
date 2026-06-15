using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimpleCommerce.BLL.Services.Implementations;
using SimpleCommerce.BLL.Services.Interfaces;
using SimpleCommerce.DAL.Context;
using SimpleCommerce.DAL.Repositories.Implementations;
using SimpleCommerce.DAL.Repositories.Interfaces;
using SimpleCommerce.Models;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // user settings
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();


// Software Authentication
// Username / Email
// password
// Mobile/Email: OTP 


// Authentciation

// tables: userInformation, role, userRole, permission, rolePermission  

// Registration: userInformation (Password -> PasswordHash -> Password Rules ())
// login: userInformation (password )

// user : permission -> 

// 100 permissions: 
// 20 user: 
// 10 roles 

// userPermission: 20 * 50 = 1000
// rolePermission: 10 * 50 = 500
// userRole: 20 * 10 = 200




// prodecut.create -> product.create, product.update, product.delete


// ID
// Name
// Email
// Department

// UserInformation

// UserId
// Name
// Email
// Department
// Password / password hash
// nasir123 -> nasir123 / hash(nasir123) -> 1234567890abcdef

// Role tabel
// -- Manager (Leave apply, Leave Approvded)
// -- Employee (Leav Apply)
// -- Admin



// 


