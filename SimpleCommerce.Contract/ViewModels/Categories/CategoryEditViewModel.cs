namespace SimpleCommerce.Contract.ViewModels.Categories;

public class CategoryEditViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
