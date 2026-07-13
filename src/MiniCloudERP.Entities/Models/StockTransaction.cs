using MiniCloudERP.Entities.Common;
using MiniCloudERP.Entities.Enums;

namespace MiniCloudERP.Entities.Models;

public class StockTransaction : BaseEntity
{
    public StockTransactionType TransactionType { get; set; }
    public int Quantity { get; set; }
    public DateTime TransactionDateUtc { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }
}
