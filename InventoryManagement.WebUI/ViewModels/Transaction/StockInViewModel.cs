using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.ViewModels.Transaction;

/// <summary>
/// ViewModel for stock in transactions
/// </summary>
public class StockInViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Product is required")]
    [Display(Name = "Product")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    [Display(Name = "Quantity to Add")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Location is required")]
    [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
    [Display(Name = "Storage Location")]
    public string Location { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    [Display(Name = "Transaction Notes")]
    public string? Notes { get; set; }

    [Display(Name = "Transaction Date")]
    [DataType(DataType.DateTime)]
    public DateTime TransactionDate { get; set; } = DateTime.Now;

    [StringLength(100, ErrorMessage = "Reference number cannot exceed 100 characters")]
    [Display(Name = "Reference Number")]
    public string? ReferenceNumber { get; set; }

    [StringLength(100, ErrorMessage = "Supplier cannot exceed 100 characters")]
    [Display(Name = "Supplier")]
    public string? Supplier { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Cost per unit must be 0 or greater")]
    [Display(Name = "Cost Per Unit")]
    [DataType(DataType.Currency)]
    public decimal? CostPerUnit { get; set; }

    [Display(Name = "Total Cost")]
    [DataType(DataType.Currency)]
    public decimal? TotalCost => CostPerUnit.HasValue ? CostPerUnit.Value * Quantity : null;

    // Product information for display
    [Display(Name = "Product Name")]
    public string ProductName { get; set; } = string.Empty;

    [Display(Name = "Current Stock")]
    public int CurrentStock { get; set; }

    [Display(Name = "Stock After Transaction")]
    public int StockAfterTransaction => CurrentStock + Quantity;

    // Navigation properties for dropdowns
    public List<SelectListItem> Products { get; set; } = new();
    public List<SelectListItem> Locations { get; set; } = new();
    public List<SelectListItem> Suppliers { get; set; } = new();

    public StockInViewModel()
    {
        PageTitle = "Stock In";
        PageSubtitle = "Add stock to inventory";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Inventory", "/Inventory"),
            ("Stock In", null)
        };
    }
}
