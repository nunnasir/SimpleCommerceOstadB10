using System.ComponentModel.DataAnnotations;

namespace SimpleCommerce.Contract.ViewModels.Products;

public class ProductEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(256, MinimumLength = 1)]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Category is required.")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    public string? ImagePath { get; set; }
}
