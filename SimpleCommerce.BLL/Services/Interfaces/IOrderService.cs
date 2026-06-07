using SimpleCommerce.Contract.ViewModels.Orders;
using SimpleCommerce.Models;

namespace SimpleCommerce.BLL.Services.Interfaces;

public interface IOrderService
{
    Task<IReadOnlyList<OrderViewModel>> GetAllAsync();
    Task<OrderViewModel?> GetByIdAsync(int id);
    Task<int?> PlaceOrderAsync(CheckoutViewModel checkout, int createdBy);
    Task UpdateStatusAsync(int id, OrderStatus status, int updatedBy);
}
