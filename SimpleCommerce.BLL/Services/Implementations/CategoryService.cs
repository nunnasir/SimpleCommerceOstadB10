using SimpleCommerce.BLL.Services.Interfaces;
using SimpleCommerce.Contract.ViewModels.Categories;
using SimpleCommerce.Contract.ViewModels.Common;
using SimpleCommerce.DAL.Repositories.Interfaces;

namespace SimpleCommerce.BLL.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public Task<IReadOnlyList<CategoryViewModel>> GetAllAsync()
    {
        // Custom Business Logic

        return _categoryRepository.GetAllAsync();
    }

    public Task<PagedResultViewModel<CategoryViewModel>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return _categoryRepository.GetPagedAsync(pageNumber, pageSize);
    }

    public Task<CategoryViewModel?> GetByIdAsync(int id)
    {
        return _categoryRepository.GetByIdAsync(id);
    }

    public Task<int> CreateAsync(CategoryCreateViewModel model, int createdByUserId)
    {
        return _categoryRepository.AddAsync(model, createdByUserId);
    }

    public Task UpdateAsync(CategoryEditViewModel model, int updatedByUserId)
    {
        return _categoryRepository.UpdateAsync(model, updatedByUserId);
    }

    public Task DeleteAsync(int id)
    {
        return _categoryRepository.DeleteAsync(id);
    }
}
