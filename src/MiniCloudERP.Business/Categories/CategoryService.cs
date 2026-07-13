using MiniCloudERP.Business.Common;
using MiniCloudERP.Entities.Abstractions;
using MiniCloudERP.Entities.Models;

namespace MiniCloudERP.Business.Categories;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        => categoryRepository.GetAllAsync(cancellationToken);

    public Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => categoryRepository.GetByIdAsync(id, cancellationToken);

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        ValidateCategory(category);

        if (await categoryRepository.ExistsByNameAsync(category.Name, cancellationToken: cancellationToken))
            throw new BusinessValidationException("A category with this name already exists.");

        category.CreatedAtUtc = DateTime.UtcNow;
        await categoryRepository.AddAsync(category, cancellationToken);
        await categoryRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        ValidateCategory(category);

        var existing = await categoryRepository.GetByIdAsync(category.Id, cancellationToken);
        if (existing is null)
            throw new BusinessValidationException("Category not found.");

        if (await categoryRepository.ExistsByNameAsync(category.Name, category.Id, cancellationToken))
            throw new BusinessValidationException("A category with this name already exists.");

        existing.Name = category.Name.Trim();
        existing.Description = category.Description?.Trim();
        existing.IsActive = category.IsActive;
        existing.UpdatedAtUtc = DateTime.UtcNow;

        categoryRepository.Update(existing);
        await categoryRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
            return;

        var productCount = await categoryRepository.GetProductCountAsync(id, cancellationToken);
        if (productCount > 0)
            throw new BusinessValidationException("Cannot delete a category that has products assigned to it.");

        categoryRepository.Delete(category);
        await categoryRepository.SaveChangesAsync(cancellationToken);
    }

    private static void ValidateCategory(Category category)
    {
        if (string.IsNullOrWhiteSpace(category.Name))
            throw new BusinessValidationException("Name is required.");
    }
}
