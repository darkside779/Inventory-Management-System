using System.ComponentModel.DataAnnotations;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.WebUI.ViewModels.Inventory;

/// <summary>
/// ViewModel for inventory listing page
/// </summary>
public class InventoryIndexViewModel
{
    /// <summary>
    /// List of inventory records for current page
    /// </summary>
    public List<InventoryDto> Inventories { get; set; } = new();
    
    /// <summary>
    /// Current page number
    /// </summary>
    public int CurrentPage { get; set; } = 1;
    
    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }
    
    /// <summary>
    /// Total count of records
    /// </summary>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; } = 10;
    
    /// <summary>
    /// Search term for filtering
    /// </summary>
    public string? SearchTerm { get; set; }
    
    /// <summary>
    /// Filter by warehouse ID
    /// </summary>
    public int? WarehouseId { get; set; }
    
    /// <summary>
    /// Filter by low stock only
    /// </summary>
    public bool? LowStockOnly { get; set; }
    
    /// <summary>
    /// Sort field
    /// </summary>
    public string SortBy { get; set; } = "ProductName";
    
    /// <summary>
    /// Sort direction
    /// </summary>
    public string SortDirection { get; set; } = "asc";
    
    /// <summary>
    /// Indicates if there are previous pages
    /// </summary>
    public bool HasPreviousPage { get; set; }
    
    /// <summary>
    /// Indicates if there are more pages
    /// </summary>
    public bool HasNextPage { get; set; }
    
    /// <summary>
    /// Page title
    /// </summary>
    public string PageTitle { get; set; } = "Inventory Management";
}


/// <summary>
/// ViewModel for stock adjustment operations
/// </summary>
public class StockAdjustmentViewModel
{
    /// <summary>
    /// Product ID
    /// </summary>
    [Required(ErrorMessage = "Product is required")]
    public int ProductId { get; set; }
    
    /// <summary>
    /// Product name (for display)
    /// </summary>
    public string ProductName { get; set; } = string.Empty;
    
    /// <summary>
    /// Warehouse ID
    /// </summary>
    [Required(ErrorMessage = "Warehouse is required")]
    public int WarehouseId { get; set; }
    
    /// <summary>
    /// Warehouse name (for display)
    /// </summary>
    public string WarehouseName { get; set; } = string.Empty;
    
    /// <summary>
    /// Current quantity in stock
    /// </summary>
    public int CurrentQuantity { get; set; }
    
    /// <summary>
    /// Adjustment quantity (positive for increase, negative for decrease)
    /// </summary>
    [Required(ErrorMessage = "Adjustment quantity is required")]
    [Display(Name = "Adjustment Quantity")]
    public int AdjustmentQuantity { get; set; }
    
    /// <summary>
    /// New quantity after adjustment
    /// </summary>
    public int NewQuantity => CurrentQuantity + AdjustmentQuantity;
    
    /// <summary>
    /// Reason for adjustment
    /// </summary>
    [Required(ErrorMessage = "Reason is required")]
    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    [Display(Name = "Reason for Adjustment")]
    public string Reason { get; set; } = string.Empty;
    
    /// <summary>
    /// Reference number
    /// </summary>
    [StringLength(50, ErrorMessage = "Reference number cannot exceed 50 characters")]
    [Display(Name = "Reference Number")]
    public string? ReferenceNumber { get; set; }
    
    /// <summary>
    /// Page title
    /// </summary>
    public string PageTitle => "Adjust Stock";
}

/// <summary>
/// ViewModel for stock transfer operations
/// </summary>
public class StockTransferViewModel
{
    /// <summary>
    /// Product ID
    /// </summary>
    [Required(ErrorMessage = "Product is required")]
    public int ProductId { get; set; }
    
    /// <summary>
    /// Product name (for display)
    /// </summary>
    public string ProductName { get; set; } = string.Empty;
    
    /// <summary>
    /// Source warehouse ID
    /// </summary>
    [Required(ErrorMessage = "Source warehouse is required")]
    [Display(Name = "Source Warehouse")]
    public int SourceWarehouseId { get; set; }
    
    /// <summary>
    /// Source warehouse name (for display)
    /// </summary>
    public string SourceWarehouseName { get; set; } = string.Empty;
    
    /// <summary>
    /// Destination warehouse ID
    /// </summary>
    [Required(ErrorMessage = "Destination warehouse is required")]
    [Display(Name = "Destination Warehouse")]
    public int DestinationWarehouseId { get; set; }
    
    /// <summary>
    /// Available quantity in source warehouse
    /// </summary>
    public int AvailableQuantity { get; set; }
    
    /// <summary>
    /// Quantity to transfer
    /// </summary>
    [Required(ErrorMessage = "Transfer quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Transfer quantity must be greater than 0")]
    [Display(Name = "Transfer Quantity")]
    public int Quantity { get; set; }
    
    /// <summary>
    /// Reason for transfer
    /// </summary>
    [Required(ErrorMessage = "Reason is required")]
    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    [Display(Name = "Reason for Transfer")]
    public string Reason { get; set; } = string.Empty;
    
    /// <summary>
    /// Reference number
    /// </summary>
    [StringLength(50, ErrorMessage = "Reference number cannot exceed 50 characters")]
    [Display(Name = "Reference Number")]
    public string? ReferenceNumber { get; set; }
    
    /// <summary>
    /// Page title
    /// </summary>
    public string PageTitle => "Transfer Stock";
}

/// <summary>
/// ViewModel for stock reservation operations
/// </summary>
public class StockReservationViewModel
{
    /// <summary>
    /// Product ID
    /// </summary>
    [Required(ErrorMessage = "Product is required")]
    public int ProductId { get; set; }
    
    /// <summary>
    /// Product name (for display)
    /// </summary>
    public string ProductName { get; set; } = string.Empty;
    
    /// <summary>
    /// Warehouse ID
    /// </summary>
    [Required(ErrorMessage = "Warehouse is required")]
    public int WarehouseId { get; set; }
    
    /// <summary>
    /// Warehouse name (for display)
    /// </summary>
    public string WarehouseName { get; set; } = string.Empty;
    
    /// <summary>
    /// Available quantity for reservation
    /// </summary>
    public int AvailableQuantity { get; set; }
    
    /// <summary>
    /// Quantity to reserve
    /// </summary>
    [Required(ErrorMessage = "Reserve quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Reserve quantity must be greater than 0")]
    [Display(Name = "Reserve Quantity")]
    public int Quantity { get; set; }
    
    /// <summary>
    /// Reason for reservation
    /// </summary>
    [Required(ErrorMessage = "Reason is required")]
    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    [Display(Name = "Reason for Reservation")]
    public string Reason { get; set; } = string.Empty;
    
    /// <summary>
    /// Reference number (e.g., Order ID)
    /// </summary>
    [StringLength(50, ErrorMessage = "Reference number cannot exceed 50 characters")]
    [Display(Name = "Reference Number")]
    public string? ReferenceNumber { get; set; }
    
    /// <summary>
    /// Page title
    /// </summary>
    public string PageTitle => "Reserve Stock";
}
