using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.ViewModels.Transaction;

/// <summary>
/// ViewModel for transaction history list with search and filtering
/// </summary>
public class TransactionListViewModel : PagedViewModel<TransactionItemViewModel>
{
    // Search and filter properties
    [Display(Name = "Search")]
    public new string? SearchTerm { get; set; }

    [Display(Name = "Product")]
    public int? ProductId { get; set; }

    [Display(Name = "Transaction Type")]
    public string? TransactionType { get; set; }

    [Display(Name = "Location")]
    public string? Location { get; set; }

    [Display(Name = "User")]
    public string? UserId { get; set; }

    [Display(Name = "Date From")]
    [DataType(DataType.Date)]
    public DateTime? DateFrom { get; set; }

    [Display(Name = "Date To")]
    [DataType(DataType.Date)]
    public DateTime? DateTo { get; set; }

    [Display(Name = "Reference Number")]
    public string? ReferenceNumber { get; set; }

    // Filter options for dropdowns
    public List<SelectListItem> Products { get; set; } = new();
    public List<SelectListItem> TransactionTypes { get; set; } = new()
    {
        new SelectListItem { Value = "", Text = "All Types" },
        new SelectListItem { Value = "StockIn", Text = "Stock In" },
        new SelectListItem { Value = "StockOut", Text = "Stock Out" },
        new SelectListItem { Value = "Adjustment", Text = "Adjustment" },
        new SelectListItem { Value = "Transfer", Text = "Transfer" }
    };
    public List<SelectListItem> Locations { get; set; } = new();
    public List<SelectListItem> Users { get; set; } = new();

    // Summary information
    [Display(Name = "Total Transactions")]
    public int TotalTransactions { get; set; }

    [Display(Name = "Stock In Transactions")]
    public int StockInCount { get; set; }

    [Display(Name = "Stock Out Transactions")]
    public int StockOutCount { get; set; }

    [Display(Name = "Total Value In")]
    [DataType(DataType.Currency)]
    public decimal TotalValueIn { get; set; }

    [Display(Name = "Total Value Out")]
    [DataType(DataType.Currency)]
    public decimal TotalValueOut { get; set; }

    public TransactionListViewModel()
    {
        PageTitle = "Transaction History";
        PageSubtitle = "View all inventory movements and transactions";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Inventory", "/Inventory"),
            ("Transactions", null)
        };
    }
}

/// <summary>
/// Individual transaction item for list display
/// </summary>
public class TransactionItemViewModel
{
    public int Id { get; set; }

    [Display(Name = "Product")]
    public string ProductName { get; set; } = string.Empty;

    [Display(Name = "SKU")]
    public string ProductSku { get; set; } = string.Empty;

    [Display(Name = "Type")]
    public string TransactionType { get; set; } = string.Empty;

    [Display(Name = "Quantity")]
    public int Quantity { get; set; }

    [Display(Name = "Location")]
    public string Location { get; set; } = string.Empty;

    [Display(Name = "Reference")]
    public string? ReferenceNumber { get; set; }

    [Display(Name = "Date")]
    [DataType(DataType.DateTime)]
    public DateTime TransactionDate { get; set; }

    [Display(Name = "User")]
    public string CreatedBy { get; set; } = string.Empty;

    [Display(Name = "Value")]
    [DataType(DataType.Currency)]
    public decimal? TransactionValue { get; set; }

    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    // Computed properties
    public bool IsStockIn => TransactionType.Contains("In", StringComparison.OrdinalIgnoreCase) || 
                            TransactionType.Equals("StockIn", StringComparison.OrdinalIgnoreCase);

    public bool IsStockOut => TransactionType.Contains("Out", StringComparison.OrdinalIgnoreCase) || 
                             TransactionType.Equals("StockOut", StringComparison.OrdinalIgnoreCase);

    public string TransactionIcon => IsStockIn ? "fas fa-arrow-up text-success" : 
                                   IsStockOut ? "fas fa-arrow-down text-danger" : "fas fa-exchange-alt text-info";

    public string TransactionBadgeClass => IsStockIn ? "badge bg-success" : 
                                         IsStockOut ? "badge bg-danger" : "badge bg-info";

    public string QuantityDisplay => IsStockIn ? $"+{Quantity}" : $"-{Quantity}";
}
