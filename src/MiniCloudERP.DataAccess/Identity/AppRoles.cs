namespace MiniCloudERP.DataAccess.Identity;

/// <summary>
/// The two basic roles supported by MiniCloudERP, per the project scope.
/// </summary>
public static class AppRoles
{
    public const string Admin = "Admin";
    public const string User = "User";

    public static readonly string[] All = [Admin, User];
}
