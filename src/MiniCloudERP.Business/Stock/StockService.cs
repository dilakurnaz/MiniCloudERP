using MiniCloudERP.Business.Common;
using MiniCloudERP.Entities.Abstractions;
using MiniCloudERP.Entities.Enums;
using MiniCloudERP.Entities.Models;

namespace MiniCloudERP.Business.Stock;

public class StockService(
    IStockRepository stockRepository,
    IProductRepository productRepository,
    IOrderRepository orderRepository) : IStockService
{
    public Task<List<StockTransaction>> GetHistoryAsync(CancellationToken cancellationToken = default)
        => stockRepository.GetHistoryAsync(cancellationToken);

    public Task IncreaseStockAsync(int productId, int quantity, string? notes = null, CancellationToken cancellationToken = default)
        => ApplyMovementAsync(productId, quantity, StockTransactionType.StockIn, notes, cancellationToken);

    public Task DecreaseStockAsync(int productId, int quantity, string? notes = null, CancellationToken cancellationToken = default)
        => ApplyMovementAsync(productId, -quantity, StockTransactionType.StockOut, notes, cancellationToken);

    public async Task AdjustStockAsync(int productId, int newQuantity, string? notes = null, CancellationToken cancellationToken = default)
    {
        if (newQuantity < 0)
            throw new BusinessValidationException("Stock quantity cannot be negative.");

        var product = await productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
            throw new BusinessValidationException("Product not found.");

        var delta = newQuantity - product.CurrentStock;
        if (delta == 0)
            return;

        product.CurrentStock = newQuantity;
        product.UpdatedAtUtc = DateTime.UtcNow;

        await stockRepository.AddTransactionAsync(new StockTransaction
        {
            ProductId = productId,
            TransactionType = StockTransactionType.Adjustment,
            Quantity = Math.Abs(delta),
            TransactionDateUtc = DateTime.UtcNow,
            Notes = notes ?? $"Adjusted stock to {newQuantity}",
            CreatedAtUtc = DateTime.UtcNow
        }, cancellationToken);

        productRepository.Update(product);
        await stockRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task ApplyOrderStockOutAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdWithDetailsAsync(orderId, cancellationToken);
        if (order is null)
            throw new BusinessValidationException("Order not found.");

        foreach (var item in order.Items)
        {
            await ApplyMovementAsync(
                item.ProductId,
                -item.Quantity,
                StockTransactionType.StockOut,
                $"Order {order.OrderNumber}",
                cancellationToken);
        }
    }

    public async Task RestoreOrderStockAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdWithDetailsAsync(orderId, cancellationToken);
        if (order is null)
            throw new BusinessValidationException("Order not found.");

        foreach (var item in order.Items)
        {
            await ApplyMovementAsync(
                item.ProductId,
                item.Quantity,
                StockTransactionType.StockIn,
                $"Cancelled order {order.OrderNumber}",
                cancellationToken);
        }
    }

    private async Task ApplyMovementAsync(
        int productId,
        int signedQuantity,
        StockTransactionType transactionType,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (signedQuantity == 0)
            throw new BusinessValidationException("Quantity must be greater than zero.");

        var product = await productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
            throw new BusinessValidationException("Product not found.");

        var newStock = product.CurrentStock + signedQuantity;
        if (newStock < 0)
            throw new BusinessValidationException(
                $"Insufficient stock for {product.Name}. Available: {product.CurrentStock}.");

        product.CurrentStock = newStock;
        product.UpdatedAtUtc = DateTime.UtcNow;

        await stockRepository.AddTransactionAsync(new StockTransaction
        {
            ProductId = productId,
            TransactionType = transactionType,
            Quantity = Math.Abs(signedQuantity),
            TransactionDateUtc = DateTime.UtcNow,
            Notes = notes,
            CreatedAtUtc = DateTime.UtcNow
        }, cancellationToken);

        productRepository.Update(product);
        await stockRepository.SaveChangesAsync(cancellationToken);
    }
}
