using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace MiniCloudERP.Web.Components.Account;

/// <summary>
/// Small helper used by the Login page to force a full-page redirect after sign-in.
/// Login.razor renders statically (no @rendermode), so a plain NavigateTo call during
/// that phase throws a NavigationException that ASP.NET Core turns into an HTTP 302 —
/// this wrapper just makes that intent explicit and reusable.
/// </summary>
public sealed class IdentityRedirectManager(NavigationManager navigationManager)
{
    [DoesNotReturn]
    public void RedirectTo(string? uri)
    {
        uri ??= "";

        // Avoid open-redirect issues by only allowing relative URIs.
        if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
        {
            uri = navigationManager.ToBaseRelativePath(uri);
        }

        navigationManager.NavigateTo(uri, forceLoad: true);
        throw new InvalidOperationException($"{nameof(IdentityRedirectManager)} can only be used during static rendering.");
    }
}
