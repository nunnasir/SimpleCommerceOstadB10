using SimpleCommerce.Contract.ViewModels.Products;

namespace SimpleCommerce.BLL.Services.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<ProductViewModel>> SearchAsync(string? searchTerm, int? categoryId);
    Task<IReadOnlyList<ProductViewModel>> GetAllAsync();
    Task<ProductViewModel?> GetByIdAsync(int id);
    Task<int> CreateAsync(ProductCreateViewModel model, int createdByUserId);
    Task UpdateAsync(ProductEditViewModel model, int updatedByUserId);
    Task DeleteAsync(int id);
}
