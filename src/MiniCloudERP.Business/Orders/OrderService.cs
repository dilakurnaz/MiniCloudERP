using MiniCloudERP.Business.Common;
using MiniCloudERP.Business.Stock;
using MiniCloudERP.Entities.Abstractions;
using MiniCloudERP.Entities.Enums;
using MiniCloudERP.Entities.Models;

namespace MiniCloudERP.Business.Orders;

public class OrderService(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IRepository<Customer> customerRepository,
    IStockService stockService) : IOrderService
{
    private static readonly HashSet<OrderStatus> StockDeductingStatuses =
    [
        OrderStatus.Confirmed,
        OrderStatus.Shipped
    ];

    public Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        => orderRepository.GetAllWithCustomerAsync(cancellationToken);

    public Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => orderRepository.GetByIdWithDetailsAsync(id, cancellationToken);

    public async Task<Order> CreateAsync(OrderCreateRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);

        var customer = await customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
            throw new BusinessValidationException("Selected customer does not exist.");

        var orderItems = new List<OrderItem>();
        foreach (var line in request.Lines)
        {
            var product = await productRepository.GetByIdAsync(line.ProductId, cancellationToken);
            if (product is null)
                throw new BusinessValidationException($"Product with id {line.ProductId} was not found.");

            if (line.Quantity <= 0)
                throw new BusinessValidationException("Order line quantity must be greater than zero.");

            if (StockDeductingStatuses.Contains(request.Status) && product.CurrentStock < line.Quantity)
                throw new BusinessValidationException(
                    $"Insufficient stock for {product.Name}. Available: {product.CurrentStock}, requested: {line.Quantity}.");

            orderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = line.Quantity,
                UnitPrice = product.SalePrice,
                LineTotal = product.SalePrice * line.Quantity,
                CreatedAtUtc = DateTime.UtcNow
            });
        }

        var order = new Order
        {
            OrderNumber = await orderRepository.GenerateOrderNumberAsync(cancellationToken),
            CustomerId = request.CustomerId,
            OrderDateUtc = DateTime.UtcNow,
            Status = request.Status,
            TotalAmount = orderItems.Sum(i => i.LineTotal),
            Items = orderItems,
            CreatedAtUtc = DateTime.UtcNow
        };

        await orderRepository.AddAsync(order, cancellationToken);
        await orderRepository.SaveChangesAsync(cancellationToken);

        if (StockDeductingStatuses.Contains(request.Status))
            await stockService.ApplyOrderStockOutAsync(order.Id, cancellationToken);

        return (await orderRepository.GetByIdWithDetailsAsync(order.Id, cancellationToken))!;
    }

    public async Task UpdateStatusAsync(int id, OrderStatus newStatus, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (order is null)
            throw new BusinessValidationException("Order not found.");

        var previousStatus = order.Status;
        if (previousStatus == newStatus)
            return;

        if (previousStatus == OrderStatus.Cancelled)
            throw new BusinessValidationException("Cancelled orders cannot be updated.");

        var wasStockDeducted = StockDeductingStatuses.Contains(previousStatus);
        var willDeductStock = StockDeductingStatuses.Contains(newStatus);

        if (!wasStockDeducted && willDeductStock)
        {
            foreach (var item in order.Items)
            {
                var product = await productRepository.GetByIdAsync(item.ProductId, cancellationToken);
                if (product is null)
                    continue;

                if (product.CurrentStock < item.Quantity)
                    throw new BusinessValidationException(
                        $"Insufficient stock for {product.Name}. Available: {product.CurrentStock}, required: {item.Quantity}.");
            }

            await stockService.ApplyOrderStockOutAsync(order.Id, cancellationToken);
        }
        else if (wasStockDeducted && newStatus == OrderStatus.Cancelled)
        {
            await stockService.RestoreOrderStockAsync(order.Id, cancellationToken);
        }

        order.Status = newStatus;
        order.UpdatedAtUtc = DateTime.UtcNow;
        orderRepository.Update(order);
        await orderRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (order is null)
            return;

        if (StockDeductingStatuses.Contains(order.Status))
            throw new BusinessValidationException("Cannot delete an order that has already affected stock. Cancel it instead.");

        orderRepository.Delete(order);
        await orderRepository.SaveChangesAsync(cancellationToken);
    }

    private static void ValidateRequest(OrderCreateRequest request)
    {
        if (request.CustomerId <= 0)
            throw new BusinessValidationException("Customer is required.");

        if (request.Lines.Count == 0)
            throw new BusinessValidationException("At least one order line is required.");
    }
}
