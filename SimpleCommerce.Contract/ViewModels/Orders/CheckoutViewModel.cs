using System.ComponentModel.DataAnnotations;

namespace SimpleCommerce.Contract.ViewModels.Orders;

public class CheckoutViewModel
{
    [Required]
    [StringLength(256)]
    [Display(Name = "Full name")]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    [Display(Name = "Email")]
    public string CustomerEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    [Display(Name = "Shipping address")]
    public string ShippingAddress { get; set; } = string.Empty;
}
