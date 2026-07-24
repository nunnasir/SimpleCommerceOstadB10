using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleCommerce.BLL.Services.Interfaces;
using SimpleCommerce.Contract.Exceptions;
using SimpleCommerce.Contract.ViewModels.Products;

namespace SimpleCommerce.API.Controllers;

[ApiVersion(1.0)]
[ApiVersion(2.0)]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private const int DefaultAuditUserId = 1;

    private readonly IProductService _productService;
    private readonly IProductImageService _productImageService;

    public ProductController(
        IProductService productService,
        IProductImageService productImageService)
    {
        _productService = productService;
        _productImageService = productImageService;
    }

    [HttpGet]
    [MapToApiVersion(1.0)]
    public async Task<ActionResult<IReadOnlyList<ProductViewModel>>> GetAll()
    {
        var items = await _productService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet]
    [MapToApiVersion(2.0)]
    public async Task<ActionResult<IReadOnlyList<ProductViewModel>>> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] int? categoryId)
    {
        var items = await _productService.SearchAsync(searchTerm, categoryId);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductViewModel>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null)
            return NotFound();

        return Ok(product);
    }

    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ProductViewModel>> Create(
        [FromForm] ProductCreateViewModel model,
        IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            if (imageFile is not null && imageFile.Length > 0)
                model.ImagePath = await _productImageService.SaveAsync(imageFile);
        }
        catch (BadRequestException ex)
        {
            ModelState.AddModelError(nameof(imageFile), ex.Message);
            return ValidationProblem(ModelState);
        }

        var id = await _productService.CreateAsync(model, DefaultAuditUserId);
        var created = await _productService.GetByIdAsync(id);

        return CreatedAtAction(nameof(GetById), new { id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(
        int id,
        [FromForm] ProductEditViewModel model,
        IFormFile? imageFile)
    {
        if (id != model.Id)
            return BadRequest("Route id does not match model id.");

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

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
        catch (BadRequestException ex)
        {
            ModelState.AddModelError(nameof(imageFile), ex.Message);
            return ValidationProblem(ModelState);
        }

        try
        {
            await _productService.UpdateAsync(model, DefaultAuditUserId);
        }
        catch (NotFoundException)
        {
            if (model.ImagePath != previousImagePath)
                _productImageService.DeleteIfExists(model.ImagePath);

            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null)
            return NotFound();

        await _productService.DeleteAsync(id);
        _productImageService.DeleteIfExists(product.ImagePath);
        return NoContent();
    }
}
