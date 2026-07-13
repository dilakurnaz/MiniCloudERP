using Microsoft.EntityFrameworkCore;
using MiniCloudERP.DataAccess.Persistence;
using MiniCloudERP.Entities.Abstractions;
using MiniCloudERP.Entities.Models;

namespace MiniCloudERP.DataAccess.Repositories;

public class OrderRepository(AppDbContext context) : IOrderRepository
{
    public Task<List<Order>> GetAllWithCustomerAsync(CancellationToken cancellationToken = default)
        => context.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .OrderByDescending(o => o.OrderDateUtc)
            .ToListAsync(cancellationToken);

    public Task<Order?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        => context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public async Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var prefix = $"ORD-{today}-";
        var countToday = await context.Orders
            .CountAsync(o => o.OrderNumber.StartsWith(prefix), cancellationToken);

        return $"{prefix}{(countToday + 1):D4}";
    }

    public Task AddAsync(Order order, CancellationToken cancellationToken = default)
        => context.Orders.AddAsync(order, cancellationToken).AsTask();

    public void Update(Order order) => context.Orders.Update(order);

    public void Delete(Order order) => context.Orders.Remove(order);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
