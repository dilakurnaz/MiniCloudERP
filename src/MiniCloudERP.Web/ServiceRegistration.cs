using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniCloudERP.Business.Categories;
using MiniCloudERP.Business.Customers;
using MiniCloudERP.Business.Dashboard;
using MiniCloudERP.Business.Orders;
using MiniCloudERP.Business.Products;
using MiniCloudERP.Business.Stock;
using MiniCloudERP.DataAccess.Identity;
using MiniCloudERP.DataAccess.Persistence;
using MiniCloudERP.DataAccess.Repositories;
using MiniCloudERP.Entities.Abstractions;
using MiniCloudERP.Web.Components.Account;

namespace MiniCloudERP.Web;

public static class ServiceRegistration
{
    public static IServiceCollection AddMiniCloudErp(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found in configuration.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddMiniCloudErpIdentity();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IStockRepository, StockRepository>();

        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IStockService, StockService>();

        return services;
    }

    private static void AddMiniCloudErpIdentity(this IServiceCollection services)
    {
        // Lets components anywhere in the tree (static or interactive) resolve the
        // current user's AuthenticationState without extra plumbing per page.
        services.AddCascadingAuthenticationState();
        services.AddScoped<IdentityRedirectManager>();

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                // Kept deliberately simple for a portfolio project: no email
                // confirmation flow, short-but-reasonable password rules.
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        // Every page requires an authenticated user unless explicitly marked
        // [AllowAnonymous] (the Login page). This is the simplest way to protect
        // the whole ERP without decorating every single page.
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });
    }
}
