using SimpleCommerce.Contract.ViewModels.Products;

namespace SimpleCommerce.DAL.Repositories.Interfaces;

public interface IProductRepository
{
    Task<IReadOnlyList<ProductViewModel>> GetAllAsync();
    Task<ProductViewModel?> GetByIdAsync(int id);
    Task<int> AddAsync(ProductCreateViewModel model, int createdBy);
    Task UpdateAsync(ProductEditViewModel model, int updatedBy);
    Task DeleteAsync(int id);
}
