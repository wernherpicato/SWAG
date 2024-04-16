using PH_Swag.Models;
using Microsoft.EntityFrameworkCore;
using PH_Swag.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();



// Inject DBContext
builder.Services.AddDbContext<PH_SwagEntity>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection"))
    );

builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();




// Session code: start
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// Session code: End

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

//app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
