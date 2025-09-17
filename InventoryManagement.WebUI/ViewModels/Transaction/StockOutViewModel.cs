using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.ViewModels.Transaction;

/// <summary>
/// ViewModel for stock out transactions
/// </summary>
public class StockOutViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Product is required")]
    [Display(Name = "Product")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    [Display(Name = "Quantity to Remove")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Location is required")]
    [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
    [Display(Name = "Source Location")]
    public string Location { get; set; } = string.Empty;

    [Required(ErrorMessage = "Reason is required")]
    [Display(Name = "Reason for Stock Out")]
    public string Reason { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    [Display(Name = "Transaction Notes")]
    public string? Notes { get; set; }

    [Display(Name = "Transaction Date")]
    [DataType(DataType.DateTime)]
    public DateTime TransactionDate { get; set; } = DateTime.Now;

    [StringLength(100, ErrorMessage = "Reference number cannot exceed 100 characters")]
    [Display(Name = "Reference Number")]
    public string? ReferenceNumber { get; set; }

    [StringLength(100, ErrorMessage = "Recipient cannot exceed 100 characters")]
    [Display(Name = "Recipient/Customer")]
    public string? Recipient { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Sale price per unit must be 0 or greater")]
    [Display(Name = "Sale Price Per Unit")]
    [DataType(DataType.Currency)]
    public decimal? SalePricePerUnit { get; set; }

    [Display(Name = "Total Sale Value")]
    [DataType(DataType.Currency)]
    public decimal? TotalSaleValue => SalePricePerUnit.HasValue ? SalePricePerUnit.Value * Quantity : null;

    // Product information for display
    [Display(Name = "Product Name")]
    public string ProductName { get; set; } = string.Empty;

    [Display(Name = "Current Stock")]
    public int CurrentStock { get; set; }

    [Display(Name = "Stock After Transaction")]
    public int StockAfterTransaction => Math.Max(0, CurrentStock - Quantity);

    [Display(Name = "Sufficient Stock")]
    public bool HasSufficientStock => CurrentStock >= Quantity;

    [Display(Name = "Stock Warning")]
    public string? StockWarning => CurrentStock < Quantity ? 
        $"Insufficient stock! Available: {CurrentStock}, Requested: {Quantity}" : null;

    // Navigation properties for dropdowns
    public List<SelectListItem> Products { get; set; } = new();
    public List<SelectListItem> Locations { get; set; } = new();
    public List<SelectListItem> Reasons { get; set; } = new()
    {
        new SelectListItem { Value = "Sale", Text = "Sale" },
        new SelectListItem { Value = "Transfer", Text = "Transfer" },
        new SelectListItem { Value = "Damaged", Text = "Damaged/Spoiled" },
        new SelectListItem { Value = "Lost", Text = "Lost/Stolen" },
        new SelectListItem { Value = "Return", Text = "Return to Supplier" },
        new SelectListItem { Value = "Adjustment", Text = "Inventory Adjustment" },
        new SelectListItem { Value = "Other", Text = "Other" }
    };

    public StockOutViewModel()
    {
        PageTitle = "Stock Out";
        PageSubtitle = "Remove stock from inventory";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Inventory", "/Inventory"),
            ("Stock Out", null)
        };
    }
}
