using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleCommerce.BLL.Services.Interfaces;
using SimpleCommerce.Contract.ViewModels.Orders;
using SimpleCommerce.Models;

namespace SimpleCommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class OrderController : Controller
{
    private const int DefaultAuditUserId = 1;

    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _orderService.GetAllAsync();
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order is null)
            return NotFound();

        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(OrderStatusUpdateViewModel model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Details), new { id = model.Id });

        try
        {
            await _orderService.UpdateStatusAsync(model.Id, model.Status, DefaultAuditUserId);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Order status updated successfully.";
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }
}
