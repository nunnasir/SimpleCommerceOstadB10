namespace SimpleCommerce.Contract.ViewModels.Cart;

public class CartViewModel
{
    public IReadOnlyList<CartItemViewModel> Items { get; set; } = [];

    public int TotalItems => Items.Sum(i => i.Quantity);

    public decimal Subtotal => Items.Sum(i => i.LineTotal);

    public bool IsEmpty => Items.Count == 0;
}
