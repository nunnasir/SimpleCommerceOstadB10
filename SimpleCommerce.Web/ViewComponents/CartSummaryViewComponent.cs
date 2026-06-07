using Microsoft.AspNetCore.Mvc;
using SimpleCommerce.BLL.Services.Interfaces;

namespace SimpleCommerce.Web.ViewComponents;

public class CartSummaryViewComponent : ViewComponent
{
    private readonly ICartService _cartService;

    public CartSummaryViewComponent(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var itemCount = await _cartService.GetItemCountAsync();
        return View(itemCount);
    }
}
