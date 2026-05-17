using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SimpleCommerce.BLL.Services.Interfaces;
using SimpleCommerce.Contract.ViewModels.Products;

namespace SimpleCommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private const int DefaultAuditUserId = 1;

    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IProductImageService _productImageService;

    public ProductController(
        IProductService productService,
        ICategoryService categoryService,
        IProductImageService productImageService)
    {
        _productService = productService;
        _categoryService = categoryService;
        _productImageService = productImageService;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _productService.GetAllAsync();
        return View(items);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null)
            return NotFound();

        return View(product);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateCategoriesAsync();
        return View(new ProductCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCreateViewModel model, IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(model.CategoryId);
            return View(model);
        }

        try
        {
            if (imageFile is not null && imageFile.Length > 0)
                model.ImagePath = await _productImageService.SaveAsync(imageFile);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(imageFile), ex.Message);
            await PopulateCategoriesAsync(model.CategoryId);
            return View(model);
        }

        await _productService.CreateAsync(model, DefaultAuditUserId);
        TempData["SuccessMessage"] = "Product created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var existing = await _productService.GetByIdAsync(id);
        if (existing is null)
            return NotFound();

        var model = new ProductEditViewModel
        {
            Id = existing.Id,
            Name = existing.Name,
            Description = existing.Description,
            Price = existing.Price,
            CategoryId = existing.CategoryId,
            ImagePath = existing.ImagePath
        };

        await PopulateCategoriesAsync(model.CategoryId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductEditViewModel model, IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(model.CategoryId);
            return View(model);
        }

        var existing = await _productService.GetByIdAsync(model.Id);
        if (existing is null)
            return NotFound();

        var previousImagePath = existing.ImagePath;

        try
        {
            if (imageFile is not null && imageFile.Length > 0)
            {
                model.ImagePath = await _productImageService.SaveAsync(imageFile);
                _productImageService.DeleteIfExists(previousImagePath);
            }
            else
            {
                model.ImagePath = previousImagePath;
            }
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(imageFile), ex.Message);
            await PopulateCategoriesAsync(model.CategoryId);
            return View(model);
        }

        try
        {
            await _productService.UpdateAsync(model, DefaultAuditUserId);
        }
        catch (InvalidOperationException)
        {
            if (model.ImagePath != previousImagePath)
                _productImageService.DeleteIfExists(model.ImagePath);

            return NotFound();
        }

        TempData["SuccessMessage"] = "Product updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null)
            return NotFound();

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null)
            return NotFound();

        await _productService.DeleteAsync(id);
        _productImageService.DeleteIfExists(product.ImagePath);
        TempData["SuccessMessage"] = "Product deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateCategoriesAsync(int? selectedCategoryId = null)
    {
        var categories = await _categoryService.GetAllAsync();
        ViewBag.CategoryId = new SelectList(categories, "Id", "Name", selectedCategoryId);
    }
}
