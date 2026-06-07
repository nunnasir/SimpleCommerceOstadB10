using SimpleCommerce.Contract.ViewModels.Cart;

namespace SimpleCommerce.Contract.ViewModels.Orders;

public class CheckoutPageViewModel
{
    public CheckoutViewModel Checkout { get; set; } = new();
    public CartViewModel Cart { get; set; } = new();
}
