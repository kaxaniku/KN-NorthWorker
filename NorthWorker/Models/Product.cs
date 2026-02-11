using System.ComponentModel.DataAnnotations;

namespace NorthWorker.Models;

public class Product
{
    public int ProductId { get; set; }
    [Required(ErrorMessage = "Name is required")]
    public string? ProductName { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Unit Price must be a non-negative integer.")]
    public decimal UnitPrice { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Units in stock must be a non-negative integer.")]
    public int UnitsInStock { get; set; }
}
