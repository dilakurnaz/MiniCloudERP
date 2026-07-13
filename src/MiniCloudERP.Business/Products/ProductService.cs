using MiniCloudERP.Business.Common;
using MiniCloudERP.Entities.Abstractions;
using MiniCloudERP.Entities.Models;

namespace MiniCloudERP.Business.Products;

public class ProductService(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository) : IProductService
{
    public Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        => productRepository.GetAllWithCategoryAsync(cancellationToken);

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => productRepository.GetByIdWithCategoryAsync(id, cancellationToken);

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await ValidateProductAsync(product, cancellationToken);

        if (await productRepository.ExistsBySkuAsync(product.Sku, cancellationToken: cancellationToken))
            throw new BusinessValidationException("A product with this SKU already exists.");

        product.Name = product.Name.Trim();
        product.Sku = product.Sku.Trim();
        product.Barcode = string.IsNullOrWhiteSpace(product.Barcode) ? null : product.Barcode.Trim();
        product.CreatedAtUtc = DateTime.UtcNow;

        await productRepository.AddAsync(product, cancellationToken);
        await productRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        await ValidateProductAsync(product, cancellationToken);

        var existing = await productRepository.GetByIdAsync(product.Id, cancellationToken);
        if (existing is null)
            throw new BusinessValidationException("Product not found.");

        if (await productRepository.ExistsBySkuAsync(product.Sku, product.Id, cancellationToken))
            throw new BusinessValidationException("A product with this SKU already exists.");

        existing.Name = product.Name.Trim();
        existing.Sku = product.Sku.Trim();
        existing.Barcode = string.IsNullOrWhiteSpace(product.Barcode) ? null : product.Barcode.Trim();
        existing.PurchasePrice = product.PurchasePrice;
        existing.SalePrice = product.SalePrice;
        existing.CurrentStock = product.CurrentStock;
        existing.CategoryId = product.CategoryId;
        existing.IsActive = product.IsActive;
        existing.UpdatedAtUtc = DateTime.UtcNow;

        productRepository.Update(existing);
        await productRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
            return;

        productRepository.Delete(product);
        await productRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateProductAsync(Product product, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
            throw new BusinessValidationException("Name is required.");

        if (string.IsNullOrWhiteSpace(product.Sku))
            throw new BusinessValidationException("SKU is required.");

        if (product.PurchasePrice < 0)
            throw new BusinessValidationException("Purchase price must be zero or greater.");

        if (product.SalePrice < 0)
            throw new BusinessValidationException("Sale price must be zero or greater.");

        if (product.CurrentStock < 0)
            throw new BusinessValidationException("Current stock must be zero or greater.");

        if (product.CategoryId <= 0)
            throw new BusinessValidationException("Category is required.");

        var category = await categoryRepository.GetByIdAsync(product.CategoryId, cancellationToken);
        if (category is null)
            throw new BusinessValidationException("Selected category does not exist.");
    }
}
