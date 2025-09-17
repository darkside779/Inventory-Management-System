using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.ViewModels.Inventory;

/// <summary>
/// ViewModel for inventory list with search and filtering
/// </summary>
public class InventoryListViewModel : PagedViewModel<InventoryItemViewModel>
{
    // Search and filter properties
    [Display(Name = "Search")]
    public new string? SearchTerm { get; set; }

    [Display(Name = "Product")]
    public int? ProductId { get; set; }

    [Display(Name = "Category")]
    public int? CategoryId { get; set; }

    [Display(Name = "Location")]
    public string? Location { get; set; }

    [Display(Name = "Transaction Type")]
    public int? TransactionType { get; set; }

    [Display(Name = "Date From")]
    [DataType(DataType.Date)]
    public DateTime? DateFrom { get; set; }

    [Display(Name = "Date To")]
    [DataType(DataType.Date)]
    public DateTime? DateTo { get; set; }

    // Filter options for dropdowns
    public List<SelectListItem> Products { get; set; } = new();
    public List<SelectListItem> Categories { get; set; } = new();
    public List<SelectListItem> Locations { get; set; } = new();
    public List<SelectListItem> TransactionTypes { get; set; } = new();

    // Summary information
    [Display(Name = "Total Transactions")]
    public int TotalTransactions { get; set; }

    [Display(Name = "Total Stock In")]
    public int TotalStockIn { get; set; }

    [Display(Name = "Total Stock Out")]
    public int TotalStockOut { get; set; }

    [Display(Name = "Net Movement")]
    public int NetMovement => TotalStockIn - TotalStockOut;

    public InventoryListViewModel()
    {
        PageTitle = "Inventory Management";
        PageSubtitle = "Track stock movements and transactions";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Inventory", null)
        };
    }
}

/// <summary>
/// Individual inventory item for list display
/// </summary>
public class InventoryItemViewModel
{
    public int Id { get; set; }

    [Display(Name = "Product")]
    public string ProductName { get; set; } = string.Empty;

    [Display(Name = "SKU")]
    public string ProductSku { get; set; } = string.Empty;

    [Display(Name = "Category")]
    public string CategoryName { get; set; } = string.Empty;

    [Display(Name = "Quantity")]
    public int Quantity { get; set; }

    [Display(Name = "Location")]
    public string Location { get; set; } = string.Empty;

    [Display(Name = "Type")]
    public string TransactionTypeName { get; set; } = string.Empty;

    [Display(Name = "Transaction Date")]
    [DataType(DataType.DateTime)]
    public DateTime TransactionDate { get; set; }

    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;

    // Computed properties for display
    [Display(Name = "Status")]
    public string Status => TransactionDate.Date == DateTime.Today.Date ? "Recent" : "Historical";

    public bool IsStockIn => TransactionTypeName.Contains("In", StringComparison.OrdinalIgnoreCase);
    public bool IsStockOut => TransactionTypeName.Contains("Out", StringComparison.OrdinalIgnoreCase);
}
