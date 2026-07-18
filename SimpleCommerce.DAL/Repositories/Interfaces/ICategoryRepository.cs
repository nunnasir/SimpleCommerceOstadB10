using SimpleCommerce.Contract.ViewModels.Categories;
using SimpleCommerce.Contract.ViewModels.Common;

namespace SimpleCommerce.DAL.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<IReadOnlyList<CategoryViewModel>> GetAllAsync();
    Task<PagedResultViewModel<CategoryViewModel>> GetPagedAsync(int pageNumber, int pageSize);
    Task<CategoryViewModel?> GetByIdAsync(int id);
    Task<int> AddAsync(CategoryCreateViewModel model, int createdBy);
    Task UpdateAsync(CategoryEditViewModel model, int updatedBy);
    Task DeleteAsync(int id);
}
