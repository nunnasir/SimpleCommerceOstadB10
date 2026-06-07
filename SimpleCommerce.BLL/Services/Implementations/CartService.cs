using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SimpleCommerce.BLL.Services.Interfaces;
using SimpleCommerce.Contract.ViewModels.Cart;

namespace SimpleCommerce.BLL.Services.Implementations;

public class CartService : ICartService
{
    private const string CartSessionKey = "Cart";

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProductService _productService;

    public CartService(IHttpContextAccessor httpContextAccessor, IProductService productService)
    {
        _httpContextAccessor = httpContextAccessor;
        _productService = productService;
    }

    public Task<CartViewModel> GetCartAsync()
    {
        var items = GetCartItems();
        return Task.FromResult(new CartViewModel { Items = items });
    }

    public Task<int> GetItemCountAsync()
    {
        var items = GetCartItems();
        return Task.FromResult(items.Sum(i => i.Quantity));
    }

    public async Task<bool> AddToCartAsync(int productId, int quantity = 1)
    {
        if (quantity < 1)
            return false;

        var product = await _productService.GetByIdAsync(productId);
        if (product is null)
            return false;

        var items = GetCartItems();
        var existingItem = items.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem is not null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            items.Add(new CartItemViewModel
            {
                ProductId = product.Id,
                Name = product.Name,
                ImagePath = product.ImagePath,
                Price = product.Price,
                Quantity = quantity
            });
        }

        SaveCartItems(items);
        return true;
    }

    public Task UpdateQuantityAsync(int productId, int quantity)
    {
        var items = GetCartItems();
        var item = items.FirstOrDefault(i => i.ProductId == productId);

        if (item is null)
            return Task.CompletedTask;

        if (quantity < 1)
        {
            items.Remove(item);
        }
        else
        {
            item.Quantity = quantity;
        }

        SaveCartItems(items);
        return Task.CompletedTask;
    }

    public Task RemoveFromCartAsync(int productId)
    {
        var items = GetCartItems();
        items.RemoveAll(i => i.ProductId == productId);
        SaveCartItems(items);
        return Task.CompletedTask;
    }

    public Task ClearCartAsync()
    {
        SaveCartItems([]);
        return Task.CompletedTask;
    }

    private List<CartItemViewModel> GetCartItems()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session is null)
            return [];

        var json = session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(json))
            return [];

        return JsonSerializer.Deserialize<List<CartItemViewModel>>(json) ?? [];
    }

    private void SaveCartItems(List<CartItemViewModel> items)
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session is null)
            return;

        session.SetString(CartSessionKey, JsonSerializer.Serialize(items));
    }
}
