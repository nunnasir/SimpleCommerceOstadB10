using ClosedXML.Excel;
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

    public async Task<byte[]> ExportToExcelAsync()
    {
        var products = await _productRepository.GetAllAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Products");

        worksheet.Cell(1, 1).Value = "Id";
        worksheet.Cell(1, 2).Value = "Name";
        worksheet.Cell(1, 3).Value = "Description";
        worksheet.Cell(1, 4).Value = "Category";
        worksheet.Cell(1, 5).Value = "Price";
        worksheet.Cell(1, 6).Value = "Image Path";
        worksheet.Cell(1, 7).Value = "Created At";
        worksheet.Cell(1, 8).Value = "Updated At";

        var headerRange = worksheet.Range(1, 1, 1, 8);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        var row = 2;
        foreach (var product in products)
        {
            worksheet.Cell(row, 1).Value = product.Id;
            worksheet.Cell(row, 2).Value = product.Name;
            worksheet.Cell(row, 3).Value = product.Description;
            worksheet.Cell(row, 4).Value = product.CategoryName;
            worksheet.Cell(row, 5).Value = product.Price;
            worksheet.Cell(row, 5).Style.NumberFormat.Format = "$#,##0.00";
            worksheet.Cell(row, 6).Value = product.ImagePath;
            worksheet.Cell(row, 7).Value = product.CreatedAt;
            worksheet.Cell(row, 7).Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
            worksheet.Cell(row, 8).Value = product.UpdatedAt;
            worksheet.Cell(row, 8).Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
