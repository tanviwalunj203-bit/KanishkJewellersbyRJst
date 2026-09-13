using KanishkJewellers.Data;
using KanishkJewellers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KanishkJewellers.Controllers;
public class JewelleryController : Controller
{
    private readonly AppDbContext _context; private readonly IWebHostEnvironment _env; private readonly string _uploadRoot;
    public JewelleryController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context; _env = env;
        var storageRoot = Environment.GetEnvironmentVariable("DATA_DIR");
        if (string.IsNullOrWhiteSpace(storageRoot)) storageRoot = Path.Combine(AppContext.BaseDirectory, "data");
        _uploadRoot = Path.Combine(storageRoot, "uploads");
        Directory.CreateDirectory(_uploadRoot);
    }
    private bool IsAdmin() => HttpContext.Session.GetString("IsAdmin") == "true";
    private IActionResult LoginRedirect() => RedirectToAction("Login", "Account")!;
    [HttpGet] public async Task<IActionResult> Index(string? category, string? search)
    {
        var q = _context.Jewellery.AsQueryable();
        if (!string.IsNullOrWhiteSpace(category)) q = q.Where(x => x.Category == category);
        if (!string.IsNullOrWhiteSpace(search)) q = q.Where(x => x.Name.Contains(search));
        ViewBag.IsAdmin = IsAdmin(); ViewBag.Category = category; ViewBag.Search = search;
        ViewBag.Categories = new[] { "Necklace", "Ring", "Earrings", "Bracelet", "Bangle", "Chain", "Pendant", "Nose Ring", "Anklet", "Mangalsutra", "Other" };
        return View(await q.OrderByDescending(x => x.Id).ToListAsync());
    }
    [HttpGet] public IActionResult Create() => IsAdmin() ? View(new Jewellery { Carat = 22 }) : LoginRedirect();
    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Jewellery jewellery, IFormFile? imageFile)
    {
        if (!IsAdmin()) return LoginRedirect();
        if (!new[] {9,18,22,24}.Contains(jewellery.Carat)) ModelState.AddModelError(nameof(jewellery.Carat), "Select 9, 18, 22 or 24 carat.");
        if (!ModelState.IsValid) return View(jewellery);
        if (imageFile is not null && imageFile.Length > 0) jewellery.ImageFileName = await SaveImage(imageFile);
        _context.Jewellery.Add(jewellery); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index));
    }
    [HttpGet] public async Task<IActionResult> Edit(int id)
    { if (!IsAdmin()) return LoginRedirect(); var item = await _context.Jewellery.FindAsync(id); return item is null ? NotFound() : View(item); }
    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Jewellery model, IFormFile? imageFile)
    {
        if (!IsAdmin()) return LoginRedirect(); if (id != model.Id) return NotFound();
        if (!new[] {9,18,22,24}.Contains(model.Carat)) ModelState.AddModelError(nameof(model.Carat), "Select 9, 18, 22 or 24 carat.");
        if (!ModelState.IsValid) return View(model);
        var item = await _context.Jewellery.FindAsync(id); if (item is null) return NotFound();
        item.Name=model.Name; item.Category=model.Category; item.Weight=model.Weight; item.Carat=model.Carat; item.Description=model.Description;
        if (imageFile is not null && imageFile.Length > 0) { DeleteImage(item.ImageFileName); item.ImageFileName=await SaveImage(imageFile); }
        await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index));
    }
    [HttpGet] public async Task<IActionResult> Delete(int id)
    { if (!IsAdmin()) return LoginRedirect(); var item=await _context.Jewellery.FindAsync(id); return item is null ? NotFound() : View(item); }
    [HttpPost, ActionName("Delete")] [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    { if (!IsAdmin()) return LoginRedirect(); var item=await _context.Jewellery.FindAsync(id); if(item is null) return NotFound(); DeleteImage(item.ImageFileName); _context.Jewellery.Remove(item); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
    private async Task<string> SaveImage(IFormFile file)
    {
        var ext=Path.GetExtension(file.FileName).ToLowerInvariant(); var allowed=new[]{".jpg",".jpeg",".png",".webp"};
        if(!allowed.Contains(ext)) throw new InvalidOperationException("Only JPG, JPEG, PNG and WEBP images are allowed.");
        var dir=_uploadRoot; Directory.CreateDirectory(dir); var name=Guid.NewGuid().ToString("N")+ext;
        await using var stream=System.IO.File.Create(Path.Combine(dir,name)); await file.CopyToAsync(stream); return name;
    }
    private void DeleteImage(string? name) { if(string.IsNullOrWhiteSpace(name)) return; var path=Path.Combine(_uploadRoot,name); if(System.IO.File.Exists(path)) System.IO.File.Delete(path); }
}
