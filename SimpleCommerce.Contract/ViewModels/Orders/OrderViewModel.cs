using SimpleCommerce.Models;

namespace SimpleCommerce.Contract.ViewModels.Orders;

public class OrderViewModel
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public decimal Subtotal { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }

    public IReadOnlyList<OrderItemViewModel> Items { get; set; } = [];

    public int TotalItems => Items.Sum(i => i.Quantity);

    public string StatusDisplayName => Status.ToString();
}
