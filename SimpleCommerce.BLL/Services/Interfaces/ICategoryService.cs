using SimpleCommerce.Contract.ViewModels.Categories;
using SimpleCommerce.Contract.ViewModels.Common;

namespace SimpleCommerce.BLL.Services.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryViewModel>> GetAllAsync();
    Task<PagedResultViewModel<CategoryViewModel>> GetPagedAsync(int pageNumber, int pageSize);
    Task<CategoryViewModel?> GetByIdAsync(int id);
    Task<int> CreateAsync(CategoryCreateViewModel model, int createdByUserId);
    Task UpdateAsync(CategoryEditViewModel model, int updatedByUserId);
    Task DeleteAsync(int id);
}
