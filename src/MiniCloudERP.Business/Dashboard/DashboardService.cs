using MiniCloudERP.Entities.Abstractions;
using MiniCloudERP.Entities.Enums;
using MiniCloudERP.Entities.Models;

namespace MiniCloudERP.Business.Dashboard;

public class DashboardService(
    IRepository<Customer> customers,
    IRepository<Product> products,
    IRepository<Supplier> suppliers,
    IRepository<Order> orders) : IDashboardService
{
    private const int LowStockThreshold = 10;

    public async Task<DashboardSummary> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var customerCount = await customers.CountAsync(cancellationToken: cancellationToken);
        var supplierCount = await suppliers.CountAsync(cancellationToken: cancellationToken);
        var orderCount = await orders.CountAsync(cancellationToken: cancellationToken);
        var lowStockProductCount = await products.CountAsync(
            product => product.CurrentStock <= LowStockThreshold,
            cancellationToken);

        // Small dataset (junior-scale ERP), so summing in-memory via the generic
        // repository is fine here instead of adding specialised aggregate queries.
        var allProducts = await products.GetAllAsync(cancellationToken);
        var productCount = allProducts.Count;
        var totalStock = allProducts.Sum(p => p.CurrentStock);

        var allOrders = await orders.GetAllAsync(cancellationToken);
        var totalSales = allOrders
            .Where(o => o.Status != OrderStatus.Cancelled)
            .Sum(o => o.TotalAmount);

        return new DashboardSummary(
            customerCount,
            productCount,
            supplierCount,
            orderCount,
            lowStockProductCount,
            totalStock,
            totalSales);
    }
}
