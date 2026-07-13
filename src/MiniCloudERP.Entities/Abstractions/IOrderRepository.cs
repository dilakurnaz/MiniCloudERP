using MiniCloudERP.Entities.Models;

namespace MiniCloudERP.Entities.Abstractions;

public interface IOrderRepository
{
    Task<List<Order>> GetAllWithCustomerAsync(CancellationToken cancellationToken = default);
    Task<Order?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    void Update(Order order);
    void Delete(Order order);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
