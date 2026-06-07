namespace SimpleCommerce.Contract.ViewModels.Cart;

public class CartItemViewModel
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public decimal LineTotal => Price * Quantity;
}
