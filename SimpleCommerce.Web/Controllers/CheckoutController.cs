using Microsoft.AspNetCore.Mvc;
using SimpleCommerce.BLL.Services.Interfaces;
using SimpleCommerce.Contract.ViewModels.Orders;

namespace SimpleCommerce.Web.Controllers;

public class CheckoutController : Controller
{
    private const int DefaultAuditUserId = 1;

    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;

    public CheckoutController(ICartService cartService, IOrderService orderService)
    {
        _cartService = cartService;
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var cart = await _cartService.GetCartAsync();
        if (cart.IsEmpty)
            return RedirectToAction("Index", "Cart");

        var model = new CheckoutPageViewModel
        {
            Cart = cart,
            Checkout = new CheckoutViewModel()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CheckoutPageViewModel model)
    {
        var cart = await _cartService.GetCartAsync();
        if (cart.IsEmpty)
            return RedirectToAction("Index", "Cart");

        model.Cart = cart;

        if (!ModelState.IsValid)
            return View(model);

        var orderId = await _orderService.PlaceOrderAsync(model.Checkout, DefaultAuditUserId);
        if (orderId is null)
            return RedirectToAction("Index", "Cart");

        return RedirectToAction(nameof(Confirmation), new { id = orderId });
    }

    [HttpGet]
    public async Task<IActionResult> Confirmation(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order is null)
            return NotFound();

        return View(order);
    }
}
