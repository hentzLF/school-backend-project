using AgriMarket.BLL.Contracts;
using AgriMarket.DAL;
using AgriMarket.DAL.Seeding;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Testcontainers.PostgreSql;

namespace AgriMarket.E2E.Infrastructure;

public sealed class E2EFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    private WebApplicationFactory<Program> _factory = null!;
    private IBrowser _browser = null!;
    private IPlaywright _playwright = null!;

    public string BaseUrl { get; private set; } = null!;
    public HttpClient HttpClient { get; private set; } = null!;

    public async Task<IBrowserContext> CreateBrowserContextAsync()
    {
        return await _browser.NewContextAsync();
    }

    public async Task<IPage> CreatePageAsync()
    {
        var context = await CreateBrowserContextAsync();
        return await context.NewPageAsync();
    }

    public async Task<IPage> CreateAuthenticatedClientPageAsync(
        string email, string password)
    {
        return await AuthHelper.LoginAsClientAsync(this, email, password);
    }

    public async Task<IPage> CreateAuthenticatedAdminPageAsync(
        string email, string password)
    {
        return await AuthHelper.LoginAsAdminAsync(this, email, password);
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor is not null)
                        services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(_postgres.GetConnectionString()));
                });
            });

        HttpClient = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        BaseUrl = _factory.Server.BaseAddress.ToString().TrimEnd('/');

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        await AppDbSeeder.SeedAsync(context, passwordHasher);

        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
    }

    public async Task DisposeAsync()
    {
        await _browser.DisposeAsync();
        _playwright.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }
}
