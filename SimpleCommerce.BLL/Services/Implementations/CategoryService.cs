using SimpleCommerce.BLL.Services.Interfaces;
using SimpleCommerce.Contract.ViewModels.Categories;
using SimpleCommerce.DAL.Repositories.Interfaces;

namespace SimpleCommerce.BLL.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public Task<IReadOnlyList<CategoryViewModel>> GetAllAsync() =>
        _categoryRepository.GetAllAsync();

    public Task<CategoryViewModel?> GetByIdAsync(int id) =>
        _categoryRepository.GetByIdAsync(id);

    public Task<int> CreateAsync(CategoryCreateViewModel model, int createdByUserId) =>
        _categoryRepository.AddAsync(model, createdByUserId);

    public Task UpdateAsync(CategoryEditViewModel model, int updatedByUserId) =>
        _categoryRepository.UpdateAsync(model, updatedByUserId);

    public Task DeleteAsync(int id) =>
        _categoryRepository.DeleteAsync(id);
}
