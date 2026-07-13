using MiniCloudERP.Entities.Enums;
using MiniCloudERP.Entities.Models;

namespace MiniCloudERP.Business.Orders;

public class OrderLineRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class OrderCreateRequest
{
    public int CustomerId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public List<OrderLineRequest> Lines { get; set; } = [];
}
