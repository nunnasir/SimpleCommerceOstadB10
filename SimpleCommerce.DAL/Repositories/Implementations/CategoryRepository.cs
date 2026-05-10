using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SimpleCommerce.Contract.ViewModels.Categories;
using SimpleCommerce.DAL.Context;
using SimpleCommerce.DAL.Repositories.Interfaces;
using SimpleCommerce.Models;

namespace SimpleCommerce.DAL.Repositories.Implementations;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _dbContext;

    public CategoryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<CategoryViewModel>> GetAllAsync()
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(MapToViewModel())
            .ToListAsync();
    }

    public async Task<CategoryViewModel?> GetByIdAsync(int id)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(MapToViewModel())
            .FirstOrDefaultAsync();
    }

    public async Task<int> AddAsync(CategoryCreateViewModel model, int createdBy)
    {
        var entity = new Category
        {
            Name = model.Name,
            Description = model.Description,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        _dbContext.Categories.Add(entity);
        await _dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(CategoryEditViewModel model, int updatedBy)
    {
        var entity = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == model.Id);
        if (entity is null)
            throw new InvalidOperationException($"Category {model.Id} was not found.");

        entity.Name = model.Name;
        entity.Description = model.Description;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = updatedBy;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (entity is null)
            return;

        _dbContext.Categories.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    private static Expression<Func<Category, CategoryViewModel>> MapToViewModel()
    {
        return c => new CategoryViewModel
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            CreatedAt = c.CreatedAt,
            CreatedBy = c.CreatedBy,
            UpdatedAt = c.UpdatedAt,
            UpdatedBy = c.UpdatedBy
        };
    }
}
