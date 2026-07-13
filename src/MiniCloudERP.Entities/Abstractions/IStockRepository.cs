using MiniCloudERP.Entities.Models;

namespace MiniCloudERP.Entities.Abstractions;

public interface IStockRepository
{
    Task<List<StockTransaction>> GetHistoryAsync(CancellationToken cancellationToken = default);
    Task AddTransactionAsync(StockTransaction transaction, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
