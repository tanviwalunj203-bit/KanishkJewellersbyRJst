using KanishkJewellers.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
var storageRoot = Environment.GetEnvironmentVariable("DATA_DIR");
if (string.IsNullOrWhiteSpace(storageRoot))
    storageRoot = Path.Combine(AppContext.BaseDirectory, "data");
Directory.CreateDirectory(storageRoot);
var uploadRoot = Path.Combine(storageRoot, "uploads");
Directory.CreateDirectory(uploadRoot);

// Keep existing local data when upgrading from older project versions.
var legacyDb = Path.Combine(AppContext.BaseDirectory, "jewelry.db");
var newDb = Path.Combine(storageRoot, "jewelry.db");
if (!File.Exists(newDb) && File.Exists(legacyDb) && !string.Equals(Path.GetFullPath(newDb), Path.GetFullPath(legacyDb), StringComparison.OrdinalIgnoreCase))
    File.Copy(legacyDb, newDb);
var legacyUploads = Path.Combine(builder.Environment.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot"), "uploads");
if (Directory.Exists(legacyUploads))
{
    foreach (var file in Directory.EnumerateFiles(legacyUploads))
    {
        var dest = Path.Combine(uploadRoot, Path.GetFileName(file));
        if (!File.Exists(dest)) File.Copy(file, dest);
    }
}

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite($"Data Source={newDb}"));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(60); options.Cookie.HttpOnly = true; options.Cookie.IsEssential = true; });

var app = builder.Build();
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadRoot),
    RequestPath = "/uploads"
});
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
using (var scope = app.Services.CreateScope()) { var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); db.Database.EnsureCreated(); }
app.Run();
