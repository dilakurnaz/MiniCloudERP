using Microsoft.EntityFrameworkCore;
using MiniCloudERP.DataAccess.Persistence;
using MiniCloudERP.Entities.Abstractions;
using MiniCloudERP.Entities.Models;

namespace MiniCloudERP.DataAccess.Repositories;

public class ProductRepository(AppDbContext context) : IProductRepository
{
    public Task<List<Product>> GetAllWithCategoryAsync(CancellationToken cancellationToken = default)
        => context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<Product?> GetByIdWithCategoryAsync(int id, CancellationToken cancellationToken = default)
        => context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<bool> ExistsBySkuAsync(string sku, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalized = sku.Trim().ToLower();
        return context.Products.AnyAsync(
            p => p.Sku.ToLower() == normalized && (excludeId == null || p.Id != excludeId),
            cancellationToken);
    }

    public Task AddAsync(Product product, CancellationToken cancellationToken = default)
        => context.Products.AddAsync(product, cancellationToken).AsTask();

    public void Update(Product product) => context.Products.Update(product);

    public void Delete(Product product) => context.Products.Remove(product);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
