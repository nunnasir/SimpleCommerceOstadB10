using Microsoft.EntityFrameworkCore;
using SimpleCommerce.Contract.Exceptions;
using SimpleCommerce.Contract.ViewModels.Cart;
using SimpleCommerce.Contract.ViewModels.Orders;
using SimpleCommerce.DAL.Context;
using SimpleCommerce.DAL.Repositories.Interfaces;
using SimpleCommerce.Models;

namespace SimpleCommerce.DAL.Repositories.Implementations;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _dbContext;

    public OrderRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<OrderViewModel>> GetAllAsync()
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .OrderByDescending(o => o.Id)
            .Select(o => new OrderViewModel
            {
                Id = o.Id,
                CustomerName = o.CustomerName,
                CustomerEmail = o.CustomerEmail,
                ShippingAddress = o.ShippingAddress,
                Status = o.Status,
                Subtotal = o.Subtotal,
                OrderDate = o.OrderDate,
                CreatedAt = o.CreatedAt,
                CreatedBy = o.CreatedBy,
                UpdatedAt = o.UpdatedAt,
                UpdatedBy = o.UpdatedBy,
                Items = o.Items.Select(i => new OrderItemViewModel
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<OrderViewModel?> GetByIdAsync(int id)
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new OrderViewModel
            {
                Id = o.Id,
                CustomerName = o.CustomerName,
                CustomerEmail = o.CustomerEmail,
                ShippingAddress = o.ShippingAddress,
                Status = o.Status,
                Subtotal = o.Subtotal,
                OrderDate = o.OrderDate,
                CreatedAt = o.CreatedAt,
                CreatedBy = o.CreatedBy,
                UpdatedAt = o.UpdatedAt,
                UpdatedBy = o.UpdatedBy,
                Items = o.Items.Select(i => new OrderItemViewModel
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<int> AddAsync(CheckoutViewModel checkout, IReadOnlyList<CartItemViewModel> cartItems, int createdBy)
    {
        var now = DateTime.UtcNow;
        var order = new Order
        {
            CustomerName = checkout.CustomerName,
            CustomerEmail = checkout.CustomerEmail,
            ShippingAddress = checkout.ShippingAddress,
            Status = OrderStatus.Pending,
            Subtotal = cartItems.Sum(i => i.LineTotal),
            OrderDate = now,
            CreatedAt = now,
            CreatedBy = createdBy,
            Items = cartItems.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.Name,
                UnitPrice = i.Price,
                Quantity = i.Quantity
            }).ToList()
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();
        return order.Id;
    }

    public async Task UpdateStatusAsync(int id, OrderStatus status, int updatedBy)
    {
        id = 100;

        var entity = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (entity is null)
            throw new NotFoundException("Order", id);

        entity.Status = status;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = updatedBy;

        await _dbContext.SaveChangesAsync();
    }
}
