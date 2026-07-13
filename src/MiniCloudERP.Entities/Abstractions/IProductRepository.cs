using MiniCloudERP.Entities.Models;

namespace MiniCloudERP.Entities.Abstractions;

public interface IProductRepository
{
    Task<List<Product>> GetAllWithCategoryAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdWithCategoryAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySkuAsync(string sku, int? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    void Update(Product product);
    void Delete(Product product);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
