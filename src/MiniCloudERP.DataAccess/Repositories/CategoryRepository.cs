using Microsoft.EntityFrameworkCore;
using MiniCloudERP.DataAccess.Persistence;
using MiniCloudERP.Entities.Abstractions;
using MiniCloudERP.Entities.Models;

namespace MiniCloudERP.DataAccess.Repositories;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        => context.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(cancellationToken);

    public Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => context.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalized = name.Trim().ToLower();
        return context.Categories.AnyAsync(
            c => c.Name.ToLower() == normalized && (excludeId == null || c.Id != excludeId),
            cancellationToken);
    }

    public Task<int> GetProductCountAsync(int categoryId, CancellationToken cancellationToken = default)
        => context.Products.CountAsync(p => p.CategoryId == categoryId, cancellationToken);

    public Task AddAsync(Category category, CancellationToken cancellationToken = default)
        => context.Categories.AddAsync(category, cancellationToken).AsTask();

    public void Update(Category category) => context.Categories.Update(category);

    public void Delete(Category category) => context.Categories.Remove(category);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
