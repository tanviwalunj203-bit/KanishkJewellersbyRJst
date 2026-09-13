using Microsoft.AspNetCore.Mvc;
namespace KanishkJewellers.Controllers;
public class HomeController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Error() => View();
}
