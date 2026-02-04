using System.ComponentModel.DataAnnotations;

namespace NorthWorker.Models;

public class ProductCreationModel
{
    [Required(ErrorMessage = "PRODUCT MUST BE NAMED!")]
    public required string ProductName { get; set; }
    public string? QuantityPerUnit { get; set; }
    public required string CategoryName { get; set; }
    public required string? SupplierName { get; set; }
    [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative value.")]
    public decimal UnitPrice { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Units in stock must be a non-negative integer.")]
    public int UnitsInStock { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Units on order must be a non-negative integer.")]
    public int UnitsOnOrder { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Reorder level must be a non-negative integer.")]
    public int ReorderLevel { get; set; }
}
