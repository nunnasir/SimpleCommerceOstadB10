using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleCommerce.BLL.Services.Interfaces;
using SimpleCommerce.Contract.ViewModels.Categories;

namespace SimpleCommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private const int DefaultAuditUserId = 1;

    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryViewModel>>> GetAll()
    {
        var items = await _categoryService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryViewModel>> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category is null)
            return NotFound();

        return Ok(category);
    }

    [HttpPost]
    //[Authorize(Roles = "Admin")]
    [Authorize]
    public async Task<ActionResult<CategoryViewModel>> Create([FromBody] CategoryCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var id = await _categoryService.CreateAsync(model, DefaultAuditUserId);
        var created = await _categoryService.GetByIdAsync(id);

        return CreatedAtAction(nameof(GetById), new { id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryEditViewModel model)
    {
        if (id != model.Id)
            return BadRequest("Route id does not match model id.");

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        await _categoryService.UpdateAsync(model, DefaultAuditUserId);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category is null)
            return NotFound();

        await _categoryService.DeleteAsync(id);
        return NoContent();
    }
}
