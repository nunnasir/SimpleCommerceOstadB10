using SimpleCommerce.Contract.ViewModels.Categories;

namespace SimpleCommerce.DAL.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<IReadOnlyList<CategoryViewModel>> GetAllAsync();
    Task<CategoryViewModel?> GetByIdAsync(int id);
    Task<int> AddAsync(CategoryCreateViewModel model, int createdBy);
    Task UpdateAsync(CategoryEditViewModel model, int updatedBy);
    Task DeleteAsync(int id);
}
