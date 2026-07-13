using System.ComponentModel.DataAnnotations;
using MiniCloudERP.Entities.Common;

namespace MiniCloudERP.Entities.Models;

public class Category : BaseEntity
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
