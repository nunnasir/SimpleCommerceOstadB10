using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SimpleCommerce.API.Controllers;
using SimpleCommerce.BLL.Services.Interfaces;
using SimpleCommerce.Contract.ViewModels.Products;

namespace SimpleCommerce.Tests.Controllers;

public class ProductControllerTests
{
    private readonly Mock<IProductService> _productServiceMock;
    private readonly Mock<IProductImageService> _productImageServiceMock;
    private readonly ProductController _sut;

    public ProductControllerTests()
    {
        _productServiceMock = new Mock<IProductService>();
        _productImageServiceMock = new Mock<IProductImageService>();
        _sut = new ProductController(_productServiceMock.Object, _productImageServiceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithProducts()
    {
        var products = new List<ProductViewModel>
        {
            new() { Id = 1, Name = "Galaxy Watch", Price = 100m, CategoryName = "Tech" }
        };

        _productServiceMock
            .Setup(s => s.GetAllAsync())
            .ReturnsAsync(products);

        var result = await _sut.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsAssignableFrom<IReadOnlyList<ProductViewModel>>(okResult.Value);
        Assert.Single(value);
        Assert.Equal("Galaxy Watch", value[0].Name);
    }

    [Fact]
    public async Task GetById_WhenProductExists_ReturnsOk()
    {
        var product = new ProductViewModel { Id = 1, Name = "Galaxy Watch", Price = 100m };

        _productServiceMock
            .Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync(product);

        var result = await _sut.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<ProductViewModel>(okResult.Value);
        Assert.Equal(1, value.Id);
    }

    [Fact]
    public async Task GetById_WhenProductMissing_ReturnsNotFound()
    {
        _productServiceMock
            .Setup(s => s.GetByIdAsync(99))
            .ReturnsAsync((ProductViewModel?)null);

        var result = await _sut.GetById(99);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_WithoutImage_ReturnsCreatedAtAction()
    {
        var model = new ProductCreateViewModel
        {
            Name = "Galaxy Watch",
            Price = 100m,
            CategoryId = 1
        };

        var created = new ProductViewModel
        {
            Id = 10,
            Name = model.Name,
            Price = model.Price,
            CategoryId = model.CategoryId
        };

        _productServiceMock
            .Setup(s => s.CreateAsync(model, 1))
            .ReturnsAsync(10);

        _productServiceMock
            .Setup(s => s.GetByIdAsync(10))
            .ReturnsAsync(created);

        var result = await _sut.Create(model, null);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(ProductController.GetById), createdResult.ActionName);
        Assert.Equal(10, createdResult.RouteValues?["id"]);
        _productImageServiceMock.Verify(s => s.SaveAsync(It.IsAny<IFormFile>()), Times.Never);
    }

    [Fact]
    public async Task Update_WhenIdsDoNotMatch_ReturnsBadRequest()
    {
        var model = new ProductEditViewModel
        {
            Id = 2,
            Name = "Galaxy Watch",
            Price = 100m,
            CategoryId = 1
        };

        var result = await _sut.Update(1, model, null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Route id does not match model id.", badRequest.Value);
    }

    [Fact]
    public async Task Update_WhenProductMissing_ReturnsNotFound()
    {
        var model = new ProductEditViewModel
        {
            Id = 99,
            Name = "Galaxy Watch",
            Price = 100m,
            CategoryId = 1
        };

        _productServiceMock
            .Setup(s => s.GetByIdAsync(99))
            .ReturnsAsync((ProductViewModel?)null);

        var result = await _sut.Update(99, model, null);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_WhenProductExists_DeletesProductAndImage()
    {
        var product = new ProductViewModel
        {
            Id = 1,
            Name = "Galaxy Watch",
            ImagePath = "images/products/watch.png"
        };

        _productServiceMock
            .Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync(product);

        _productServiceMock
            .Setup(s => s.DeleteAsync(1))
            .Returns(Task.CompletedTask);

        var result = await _sut.Delete(1);

        Assert.IsType<NoContentResult>(result);
        _productServiceMock.Verify(s => s.DeleteAsync(1), Times.Once);
        _productImageServiceMock.Verify(s => s.DeleteIfExists("images/products/watch.png"), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenProductMissing_ReturnsNotFound()
    {
        _productServiceMock
            .Setup(s => s.GetByIdAsync(99))
            .ReturnsAsync((ProductViewModel?)null);

        var result = await _sut.Delete(99);

        Assert.IsType<NotFoundResult>(result);
        _productServiceMock.Verify(s => s.DeleteAsync(It.IsAny<int>()), Times.Never);
    }
}
