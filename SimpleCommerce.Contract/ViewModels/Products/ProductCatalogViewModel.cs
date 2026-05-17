using SimpleCommerce.Contract.ViewModels.Categories;

namespace SimpleCommerce.Contract.ViewModels.Products;

public class ProductCatalogViewModel
{
    public IReadOnlyList<ProductViewModel> Products { get; set; } = Array.Empty<ProductViewModel>();
    public IReadOnlyList<CategoryViewModel> Categories { get; set; } = Array.Empty<CategoryViewModel>();
    public string? SearchTerm { get; set; }
    public int? SelectedCategoryId { get; set; }
}
