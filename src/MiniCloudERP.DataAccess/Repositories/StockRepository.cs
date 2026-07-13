using Microsoft.EntityFrameworkCore;
using MiniCloudERP.DataAccess.Persistence;
using MiniCloudERP.Entities.Abstractions;
using MiniCloudERP.Entities.Models;

namespace MiniCloudERP.DataAccess.Repositories;

public class StockRepository(AppDbContext context) : IStockRepository
{
    public Task<List<StockTransaction>> GetHistoryAsync(CancellationToken cancellationToken = default)
        => context.StockTransactions
            .AsNoTracking()
            .Include(t => t.Product)
            .OrderByDescending(t => t.TransactionDateUtc)
            .ToListAsync(cancellationToken);

    public Task AddTransactionAsync(StockTransaction transaction, CancellationToken cancellationToken = default)
        => context.StockTransactions.AddAsync(transaction, cancellationToken).AsTask();

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
