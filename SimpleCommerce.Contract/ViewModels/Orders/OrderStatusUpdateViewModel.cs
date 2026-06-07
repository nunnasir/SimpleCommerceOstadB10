using System.ComponentModel.DataAnnotations;
using SimpleCommerce.Models;

namespace SimpleCommerce.Contract.ViewModels.Orders;

public class OrderStatusUpdateViewModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Order status")]
    public OrderStatus Status { get; set; }
}
