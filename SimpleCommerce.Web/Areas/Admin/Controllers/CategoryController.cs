using Microsoft.AspNetCore.Mvc;
using SimpleCommerce.BLL.Services.Interfaces;
using SimpleCommerce.Contract.ViewModels.Categories;

namespace SimpleCommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{
    private const int DefaultAuditUserId = 1;

    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _categoryService.GetAllAsync();
        return View(items);
    }

    public async Task<IActionResult> Details(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category is null)
            return NotFound();

        return View(category);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CategoryCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await _categoryService.CreateAsync(model, DefaultAuditUserId);
        TempData["SuccessMessage"] = "Category created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var existing = await _categoryService.GetByIdAsync(id);
        if (existing is null)
            return NotFound();

        var model = new CategoryEditViewModel
        {
            Id = existing.Id,
            Name = existing.Name,
            Description = existing.Description
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CategoryEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _categoryService.UpdateAsync(model, DefaultAuditUserId);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Category updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category is null)
            return NotFound();

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category is null)
            return NotFound();

        await _categoryService.DeleteAsync(id);
        TempData["SuccessMessage"] = "Category deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
