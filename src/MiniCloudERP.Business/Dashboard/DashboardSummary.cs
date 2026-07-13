namespace MiniCloudERP.Business.Dashboard;

public sealed record DashboardSummary(
    int CustomerCount,
    int ProductCount,
    int SupplierCount,
    int OrderCount,
    int LowStockProductCount,
    int TotalStock,
    decimal TotalSales);
