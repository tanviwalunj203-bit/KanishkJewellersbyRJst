using System.ComponentModel.DataAnnotations;

namespace KanishkJewellers.Models;

public class Jewellery
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(50)] public string Category { get; set; } = string.Empty;
    [Range(0.001, 100000)] public decimal Weight { get; set; }
    [Range(9, 24)] public int Carat { get; set; } = 22;
    public string? ImageFileName { get; set; }
    [StringLength(1000)] public string? Description { get; set; }
}
