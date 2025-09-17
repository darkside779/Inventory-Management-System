using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.ViewModels.Inventory;

/// <summary>
/// ViewModel for editing inventory records
/// </summary>
public class InventoryEditViewModel : BaseViewModel
{
    [Required]
    public int Id { get; set; }

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
    public DateTime TransactionDate { get; set; }

    // Read-only properties for display
    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;

    [Display(Name = "Created Date")]
    [DataType(DataType.DateTime)]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Last Modified")]
    [DataType(DataType.DateTime)]
    public DateTime? LastModified { get; set; }

    // Navigation properties for dropdowns
    public List<SelectListItem> Products { get; set; } = new();
    public List<SelectListItem> TransactionTypes { get; set; } = new();
    public List<SelectListItem> Locations { get; set; } = new();

    public InventoryEditViewModel()
    {
        PageTitle = "Edit Inventory Transaction";
        PageSubtitle = "Modify stock movement record";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Inventory", "/Inventory"),
            ("Edit Transaction", null)
        };
    }
}
