using SimpleCommerce.Contract.ViewModels.Categories;

namespace SimpleCommerce.BLL.Services.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryViewModel>> GetAllAsync();
    Task<CategoryViewModel?> GetByIdAsync(int id);
    Task<int> CreateAsync(CategoryCreateViewModel model, int createdByUserId);
    Task UpdateAsync(CategoryEditViewModel model, int updatedByUserId);
    Task DeleteAsync(int id);
}
