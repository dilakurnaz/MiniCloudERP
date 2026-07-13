using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniCloudERP.DataAccess.Identity;
using MiniCloudERP.Web.Components;
using MiniCloudERP.Web;
using MiniCloudERP.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMiniCloudErp(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// Applies pending EF Core migrations and seeds baseline roles/demo users so the
// app works immediately after cloning the repo (any environment, not just Dev,
// since this is a portfolio demo rather than a production deployment).
await DevelopmentDatabaseInitializer.InitializeAsync(app.Services);

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Minimal API endpoint for logging out. Kept outside the Razor component tree
// because sign-out needs to write directly to the HTTP response (clearing the
// auth cookie), which a plain POST-ed HTML <form> handles cleanly.
app.MapPost("/Account/Logout", async (
    SignInManager<ApplicationUser> signInManager,
    [FromForm] string returnUrl) =>
{
    await signInManager.SignOutAsync();
    return TypedResults.LocalRedirect($"~/{returnUrl.TrimStart('/')}");
});

app.Run();
