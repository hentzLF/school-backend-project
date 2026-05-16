using AgriMarket.BLL;
using AgriMarket.DAL;
using AgriMarket.DAL.Seeding;
using AgriMarket.Resources;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
builder.Services.AddLocalization();
builder.Services.AddDal();
builder.Services.AddBll();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("et") };
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
});

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

    options.AddPolicy("ProviderOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Provider"));

    options.AddPolicy("ClientOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Farmer", "Provider"));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<AgriMarket.BLL.Contracts.IPasswordHasher>();
    await AppDbSeeder.SeedAsync(context, passwordHasher);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.UseRouting();
app.UseRequestLocalization();

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
