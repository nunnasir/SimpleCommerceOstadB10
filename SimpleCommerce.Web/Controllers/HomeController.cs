using Microsoft.AspNetCore.Mvc;
using SimpleCommerce.BLL.Services.Interfaces;
using SimpleCommerce.Contract.ViewModels.Products;
using SimpleCommerce.Web.Models;
using System.Diagnostics;

namespace SimpleCommerce.Web.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public HomeController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, int? categoryId)
    {
        var products = await _productService.SearchAsync(search, categoryId);
        var categories = await _categoryService.GetAllAsync();

        var model = new ProductCatalogViewModel
        {
            Products = products,
            Categories = categories,
            SearchTerm = search,
            SelectedCategoryId = categoryId
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
