using MiniCloudERP.Business.Orders;
using MiniCloudERP.Entities.Enums;
using MiniCloudERP.Entities.Models;

namespace MiniCloudERP.Business.Orders;

public interface IOrderService
{
    Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Order> CreateAsync(OrderCreateRequest request, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(int id, OrderStatus newStatus, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
