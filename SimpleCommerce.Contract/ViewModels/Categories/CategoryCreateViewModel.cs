using System.ComponentModel.DataAnnotations;

namespace SimpleCommerce.Contract.ViewModels.Categories;

public class CategoryCreateViewModel
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(256, MinimumLength = 1)]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    [Display(Name = "Description")]
    public string? Description { get; set; }
}
