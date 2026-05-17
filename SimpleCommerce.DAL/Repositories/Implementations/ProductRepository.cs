using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SimpleCommerce.Contract.ViewModels.Products;
using SimpleCommerce.DAL.Context;
using SimpleCommerce.DAL.Repositories.Interfaces;
using SimpleCommerce.Models;

namespace SimpleCommerce.DAL.Repositories.Implementations;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _dbContext;

    public ProductRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ProductViewModel>> SearchAsync(string? searchTerm, int? categoryId)
    {
        var query = _dbContext.Products.AsNoTracking();

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(p =>
                p.Name.Contains(term) ||
                (p.Description != null && p.Description.Contains(term)));
        }

        return await query
            .OrderByDescending(p => p.Id)
            .Select(MapToViewModel())
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ProductViewModel>> GetAllAsync()
    {
        return await SearchAsync(null, null);
    }

    public async Task<ProductViewModel?> GetByIdAsync(int id)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(MapToViewModel())
            .FirstOrDefaultAsync();
    }

    public async Task<int> AddAsync(ProductCreateViewModel model, int createdBy)
    {
        if (!await _dbContext.Categories.AnyAsync(c => c.Id == model.CategoryId))
            throw new InvalidOperationException($"Category {model.CategoryId} was not found.");

        var entity = new Product
        {
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            ImagePath = model.ImagePath,
            CategoryId = model.CategoryId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        _dbContext.Products.Add(entity);
        await _dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(ProductEditViewModel model, int updatedBy)
    {
        var entity = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == model.Id);
        if (entity is null)
            throw new InvalidOperationException($"Product {model.Id} was not found.");

        if (!await _dbContext.Categories.AnyAsync(c => c.Id == model.CategoryId))
            throw new InvalidOperationException($"Category {model.CategoryId} was not found.");

        entity.Name = model.Name;
        entity.Description = model.Description;
        entity.Price = model.Price;
        entity.CategoryId = model.CategoryId;
        entity.ImagePath = model.ImagePath;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = updatedBy;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (entity is null)
            return;

        _dbContext.Products.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    private static Expression<Func<Product, ProductViewModel>> MapToViewModel()
    {
        return p => new ProductViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            ImagePath = p.ImagePath,
            CategoryId = p.CategoryId,
            CategoryName = p.Category.Name,
            CreatedAt = p.CreatedAt,
            CreatedBy = p.CreatedBy,
            UpdatedAt = p.UpdatedAt,
            UpdatedBy = p.UpdatedBy
        };
    }
}
