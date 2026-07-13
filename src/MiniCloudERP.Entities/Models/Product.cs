using System.ComponentModel.DataAnnotations;
using MiniCloudERP.Entities.Common;

namespace MiniCloudERP.Entities.Models;

public class Product : BaseEntity
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(150, ErrorMessage = "Name cannot exceed 150 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "SKU is required.")]
    [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters.")]
    public string Sku { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Barcode cannot exceed 50 characters.")]
    public string? Barcode { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Purchase price must be zero or greater.")]
    public decimal PurchasePrice { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Sale price must be zero or greater.")]
    public decimal SalePrice { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Current stock must be zero or greater.")]
    public int CurrentStock { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Category is required.")]
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
}
