using Microsoft.AspNetCore.Mvc;

namespace KanishkJewellers.Controllers;

public class AccountController : Controller
{
    private static readonly Dictionary<string, string> Admins = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Shiva"] = "2000",
        ["Tanvi"] = "1186"
    };

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(string username, string password)
    {
        username = username?.Trim() ?? string.Empty;
        password ??= string.Empty;

        if (Admins.TryGetValue(username, out var expectedPassword) && password == expectedPassword)
        {
            HttpContext.Session.SetString("IsAdmin", "true");
            HttpContext.Session.SetString("AdminName", username);
            return RedirectToAction("Index", "Jewellery");
        }

        ViewBag.Error = "Wrong admin name or password.";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
