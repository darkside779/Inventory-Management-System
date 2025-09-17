using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.ViewModels.Inventory;

/// <summary>
/// ViewModel for creating new inventory records
/// </summary>
public class InventoryCreateViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Product is required")]
    [Display(Name = "Product")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    [Display(Name = "Quantity")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Location is required")]
    [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
    [Display(Name = "Location")]
    public string Location { get; set; } = string.Empty;

    [Required(ErrorMessage = "Transaction type is required")]
    [Display(Name = "Transaction Type")]
    public int TransactionType { get; set; }

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    [Display(Name = "Transaction Date")]
    [DataType(DataType.DateTime)]
    public DateTime TransactionDate { get; set; } = DateTime.Now;

    // Navigation properties for dropdowns
    public List<SelectListItem> Products { get; set; } = new();
    public List<SelectListItem> TransactionTypes { get; set; } = new();
    public List<SelectListItem> Locations { get; set; } = new();

    public InventoryCreateViewModel()
    {
        PageTitle = "Add Inventory Transaction";
        PageSubtitle = "Record new stock movement";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Inventory", "/Inventory"),
            ("Add Transaction", null)
        };
    }
}
