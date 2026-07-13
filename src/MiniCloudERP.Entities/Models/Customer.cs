using MiniCloudERP.Entities.Common;

namespace MiniCloudERP.Entities.Models;

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? TaxNumber { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
