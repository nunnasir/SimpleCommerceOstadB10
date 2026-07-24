using Moq;
using SimpleCommerce.BLL.Services.Implementations;
using SimpleCommerce.Contract.ViewModels.Products;
using SimpleCommerce.DAL.Repositories.Interfaces;

namespace SimpleCommerce.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _sut = new ProductService(_productRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsProductsFromRepository()
    {
        var expected = new List<ProductViewModel>
        {
            new() { Id = 1, Name = "Galaxy Watch", Price = 100m, CategoryName = "Tech" }
        };

        _productRepositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(expected);

        var result = await _sut.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("Galaxy Watch", result[0].Name);
        _productRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsProduct()
    {
        var expected = new ProductViewModel { Id = 1, Name = "Galaxy Watch", Price = 100m };

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(expected);

        var result = await _sut.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal("Galaxy Watch", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductMissing_ReturnsNull()
    {
        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((ProductViewModel?)null);

        var result = await _sut.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CallsRepositoryAndReturnsId()
    {
        var model = new ProductCreateViewModel
        {
            Name = "Galaxy Watch",
            Price = 100m,
            CategoryId = 1
        };

        _productRepositoryMock
            .Setup(r => r.AddAsync(model, 1))
            .ReturnsAsync(10);

        var id = await _sut.CreateAsync(model, 1);

        Assert.Equal(10, id);
        _productRepositoryMock.Verify(r => r.AddAsync(model, 1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_CallsRepository()
    {
        _productRepositoryMock
            .Setup(r => r.DeleteAsync(1))
            .Returns(Task.CompletedTask);

        await _sut.DeleteAsync(1);

        _productRepositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task ExportToExcelAsync_ReturnsExcelFileBytes()
    {
        var products = new List<ProductViewModel>
        {
            new()
            {
                Id = 1,
                Name = "Galaxy Watch",
                Description = "Smart watch",
                CategoryName = "Tech",
                Price = 100m,
                ImagePath = "images/products/watch.png",
                CreatedAt = new DateTime(2026, 6, 21, 23, 5, 0)
            }
        };

        _productRepositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(products);

        var result = await _sut.ExportToExcelAsync();

        Assert.NotEmpty(result);
        // XLSX files are ZIP packages and start with PK
        Assert.Equal((byte)'P', result[0]);
        Assert.Equal((byte)'K', result[1]);
    }
}
