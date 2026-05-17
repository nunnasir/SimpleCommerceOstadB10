using SimpleCommerce.BLL.Services.Interfaces;
using SimpleCommerce.Contract.ViewModels.Products;
using SimpleCommerce.DAL.Repositories.Interfaces;

namespace SimpleCommerce.BLL.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public Task<IReadOnlyList<ProductViewModel>> SearchAsync(string? searchTerm, int? categoryId) =>
        _productRepository.SearchAsync(searchTerm, categoryId);

    public Task<IReadOnlyList<ProductViewModel>> GetAllAsync() =>
        _productRepository.GetAllAsync();

    public Task<ProductViewModel?> GetByIdAsync(int id) =>
        _productRepository.GetByIdAsync(id);

    public Task<int> CreateAsync(ProductCreateViewModel model, int createdByUserId) =>
        _productRepository.AddAsync(model, createdByUserId);

    public Task UpdateAsync(ProductEditViewModel model, int updatedByUserId) =>
        _productRepository.UpdateAsync(model, updatedByUserId);

    public Task DeleteAsync(int id) =>
        _productRepository.DeleteAsync(id);
}
