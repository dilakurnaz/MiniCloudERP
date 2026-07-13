using MiniCloudERP.Entities.Enums;
using MiniCloudERP.Entities.Models;

namespace MiniCloudERP.Business.Stock;

public interface IStockService
{
    Task<List<StockTransaction>> GetHistoryAsync(CancellationToken cancellationToken = default);
    Task IncreaseStockAsync(int productId, int quantity, string? notes = null, CancellationToken cancellationToken = default);
    Task DecreaseStockAsync(int productId, int quantity, string? notes = null, CancellationToken cancellationToken = default);
    Task AdjustStockAsync(int productId, int newQuantity, string? notes = null, CancellationToken cancellationToken = default);
    Task ApplyOrderStockOutAsync(int orderId, CancellationToken cancellationToken = default);
    Task RestoreOrderStockAsync(int orderId, CancellationToken cancellationToken = default);
}
