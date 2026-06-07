using SimpleCommerce.BLL.Services.Interfaces;
using SimpleCommerce.Contract.ViewModels.Orders;
using SimpleCommerce.DAL.Repositories.Interfaces;
using SimpleCommerce.Models;

namespace SimpleCommerce.BLL.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartService _cartService;

    public OrderService(IOrderRepository orderRepository, ICartService cartService)
    {
        _orderRepository = orderRepository;
        _cartService = cartService;
    }

    public Task<IReadOnlyList<OrderViewModel>> GetAllAsync() =>
        _orderRepository.GetAllAsync();

    public Task<OrderViewModel?> GetByIdAsync(int id) =>
        _orderRepository.GetByIdAsync(id);

    public async Task<int?> PlaceOrderAsync(CheckoutViewModel checkout, int createdBy)
    {
        var cart = await _cartService.GetCartAsync();
        if (cart.IsEmpty)
            return null;

        var orderId = await _orderRepository.AddAsync(checkout, cart.Items, createdBy);
        await _cartService.ClearCartAsync();
        return orderId;
    }

    public Task UpdateStatusAsync(int id, OrderStatus status, int updatedBy) =>
        _orderRepository.UpdateStatusAsync(id, status, updatedBy);
}
