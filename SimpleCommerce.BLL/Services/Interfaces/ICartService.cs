using SimpleCommerce.Contract.ViewModels.Cart;

namespace SimpleCommerce.BLL.Services.Interfaces;

public interface ICartService
{
    Task<CartViewModel> GetCartAsync();
    Task<int> GetItemCountAsync();
    Task<bool> AddToCartAsync(int productId, int quantity = 1);
    Task UpdateQuantityAsync(int productId, int quantity);
    Task RemoveFromCartAsync(int productId);
    Task ClearCartAsync();
}
