using SimpleCommerce.Contract.ViewModels.Cart;
using SimpleCommerce.Contract.ViewModels.Orders;
using SimpleCommerce.Models;

namespace SimpleCommerce.DAL.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<IReadOnlyList<OrderViewModel>> GetAllAsync();
    Task<OrderViewModel?> GetByIdAsync(int id);
    Task<int> AddAsync(CheckoutViewModel checkout, IReadOnlyList<CartItemViewModel> cartItems, int createdBy);
    Task UpdateStatusAsync(int id, OrderStatus status, int updatedBy);
}
