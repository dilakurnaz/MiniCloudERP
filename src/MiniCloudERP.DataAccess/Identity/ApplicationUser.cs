using Microsoft.AspNetCore.Identity;

namespace MiniCloudERP.DataAccess.Identity;

/// <summary>
/// Application user for ASP.NET Core Identity.
/// Extends the built-in IdentityUser with a display name used across the UI.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
