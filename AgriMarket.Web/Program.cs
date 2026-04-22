using AgriMarket.DAL;
using AgriMarket.DAL.Seeding;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Client/Account/Login";
        options.AccessDeniedPath = "/Client/Account/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
        {
            OnRedirectToLogin = ctx =>
            {
                var redirect = ctx.Request.Path.StartsWithSegments("/Admin")
                    ? "/Admin/Account/Login"
                    : "/Client/Account/Login";
                ctx.Response.Redirect(redirect + ctx.Request.QueryString);
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = ctx =>
            {
                var redirect = ctx.Request.Path.StartsWithSegments("/Admin")
                    ? "/Admin/Account/AccessDenied"
                    : "/Client/Account/AccessDenied";
                ctx.Response.Redirect(redirect);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Admin"));

    options.AddPolicy("ClientOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Farmer", "Provider"));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await AppDbSeeder.SeedAsync(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
    name: "client",
    areaName: "Client",
    pattern: "Client/{controller=Listings}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
