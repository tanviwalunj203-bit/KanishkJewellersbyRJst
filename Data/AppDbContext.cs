using KanishkJewellers.Models;
using Microsoft.EntityFrameworkCore;

namespace KanishkJewellers.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Jewellery> Jewellery { get; set; } = null!;
    }
}