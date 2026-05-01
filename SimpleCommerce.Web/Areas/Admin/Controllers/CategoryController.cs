using Microsoft.AspNetCore.Mvc;

namespace SimpleCommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
