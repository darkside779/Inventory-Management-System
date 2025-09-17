using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.WebUI.ViewModels.Inventory;

/// <summary>
/// ViewModel for displaying inventory record details
/// </summary>
public class InventoryDetailsViewModel : BaseViewModel
{
    public int Id { get; set; }

    [Display(Name = "Product")]
    public string ProductName { get; set; } = string.Empty;

    [Display(Name = "Product SKU")]
    public string ProductSku { get; set; } = string.Empty;

    [Display(Name = "Category")]
    public string CategoryName { get; set; } = string.Empty;

    [Display(Name = "Quantity")]
    public int Quantity { get; set; }

    [Display(Name = "Location")]
    public string Location { get; set; } = string.Empty;

    [Display(Name = "Transaction Type")]
    public string TransactionTypeName { get; set; } = string.Empty;

    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    [Display(Name = "Transaction Date")]
    [DataType(DataType.DateTime)]
    public DateTime TransactionDate { get; set; }

    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;

    [Display(Name = "Created Date")]
    [DataType(DataType.DateTime)]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Last Modified")]
    [DataType(DataType.DateTime)]
    public DateTime? LastModified { get; set; }

    [Display(Name = "Modified By")]
    public string? ModifiedBy { get; set; }

    // Additional calculated properties
    [Display(Name = "Days Since Transaction")]
    public int DaysSinceTransaction => (DateTime.Now - TransactionDate).Days;

    [Display(Name = "Is Recent")]
    public bool IsRecentTransaction => DaysSinceTransaction <= 7;

    public InventoryDetailsViewModel()
    {
        PageTitle = "Inventory Transaction Details";
        PageSubtitle = "View stock movement information";
        BreadcrumbItems = new List<(string text, string? url)>
        {
            ("Inventory", "/Inventory"),
            ("Transaction Details", null)
        };
    }
}
