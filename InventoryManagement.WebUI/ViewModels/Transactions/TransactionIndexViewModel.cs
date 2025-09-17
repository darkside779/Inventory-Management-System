using System.ComponentModel.DataAnnotations;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.ViewModels.Transactions;

/// <summary>
/// ViewModel for the Transaction Index page
/// </summary>
public class TransactionIndexViewModel
{
    /// <summary>
    /// List of transactions
    /// </summary>
    public IEnumerable<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();

    /// <summary>
    /// Filter criteria
    /// </summary>
    public TransactionFilterViewModel Filter { get; set; } = new();

    /// <summary>
    /// Pagination information
    /// </summary>
    public PaginationViewModel Pagination { get; set; } = new();

    /// <summary>
    /// Available products for filtering
    /// </summary>
    public SelectList Products { get; set; } = new(new List<SelectListItem>(), "Value", "Text");

    /// <summary>
    /// Available warehouses for filtering
    /// </summary>
    public SelectList Warehouses { get; set; } = new(new List<SelectListItem>(), "Value", "Text");

    /// <summary>
    /// Available transaction types for filtering
    /// </summary>
    public SelectList TransactionTypes { get; set; } = new(new List<SelectListItem>(), "Value", "Text");

    /// <summary>
    /// Success message
    /// </summary>
    public string? SuccessMessage { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// ViewModel for transaction filtering
/// </summary>
public class TransactionFilterViewModel
{
    /// <summary>
    /// Search term
    /// </summary>
    [Display(Name = "Search")]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Product filter
    /// </summary>
    [Display(Name = "Product")]
    public int? ProductId { get; set; }

    /// <summary>
    /// Warehouse filter
    /// </summary>
    [Display(Name = "Warehouse")]
    public int? WarehouseId { get; set; }

    /// <summary>
    /// Transaction type filter
    /// </summary>
    [Display(Name = "Transaction Type")]
    public TransactionType? TransactionType { get; set; }

    /// <summary>
    /// Start date filter
    /// </summary>
    [Display(Name = "From Date")]
    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date filter
    /// </summary>
    [Display(Name = "To Date")]
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Sort by field
    /// </summary>
    public string? SortBy { get; set; } = "Timestamp";

    /// <summary>
    /// Sort direction
    /// </summary>
    public string? SortDirection { get; set; } = "desc";
}

/// <summary>
/// ViewModel for pagination
/// </summary>
public class PaginationViewModel
{
    /// <summary>
    /// Current page number
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Total count of items
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Has next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Has previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Start item number
    /// </summary>
    public int StartItem => (PageNumber - 1) * PageSize + 1;

    /// <summary>
    /// End item number
    /// </summary>
    public int EndItem => Math.Min(PageNumber * PageSize, TotalCount);
}

/// <summary>
/// ViewModel for creating stock in transactions
/// </summary>
public class CreateStockInViewModel
{
    /// <summary>
    /// Product ID
    /// </summary>
    [Required]
    [Display(Name = "Product")]
    public int ProductId { get; set; }

    /// <summary>
    /// Warehouse ID
    /// </summary>
    [Required]
    [Display(Name = "Warehouse")]
    public int WarehouseId { get; set; }

    /// <summary>
    /// Quantity to add
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
    [Display(Name = "Quantity")]
    public int Quantity { get; set; }

    /// <summary>
    /// Unit cost
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Unit cost cannot be negative")]
    [Display(Name = "Unit Cost")]
    [DataType(DataType.Currency)]
    public decimal? UnitCost { get; set; }

    /// <summary>
    /// Reason for stock in
    /// </summary>
    [MaxLength(200)]
    [Display(Name = "Reason")]
    public string? Reason { get; set; }

    /// <summary>
    /// Reference number
    /// </summary>
    [MaxLength(50)]
    [Display(Name = "Reference Number")]
    public string? ReferenceNumber { get; set; }

    /// <summary>
    /// Notes
    /// </summary>
    [MaxLength(500)]
    [Display(Name = "Notes")]
    [DataType(DataType.MultilineText)]
    public string? Notes { get; set; }

    /// <summary>
    /// Available products
    /// </summary>
    public SelectList Products { get; set; } = new(new List<SelectListItem>(), "Value", "Text");

    /// <summary>
    /// Available warehouses
    /// </summary>
    public SelectList Warehouses { get; set; } = new(new List<SelectListItem>(), "Value", "Text");
}

/// <summary>
/// ViewModel for creating stock out transactions
/// </summary>
public class CreateStockOutViewModel
{
    /// <summary>
    /// Product ID
    /// </summary>
    [Required]
    [Display(Name = "Product")]
    public int ProductId { get; set; }

    /// <summary>
    /// Warehouse ID
    /// </summary>
    [Required]
    [Display(Name = "Warehouse")]
    public int WarehouseId { get; set; }

    /// <summary>
    /// Quantity to remove
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
    [Display(Name = "Quantity")]
    public int Quantity { get; set; }

    /// <summary>
    /// Unit cost
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Unit cost cannot be negative")]
    [Display(Name = "Unit Cost")]
    [DataType(DataType.Currency)]
    public decimal? UnitCost { get; set; }

    /// <summary>
    /// Reason for stock out
    /// </summary>
    [MaxLength(200)]
    [Display(Name = "Reason")]
    public string? Reason { get; set; }

    /// <summary>
    /// Reference number
    /// </summary>
    [MaxLength(50)]
    [Display(Name = "Reference Number")]
    public string? ReferenceNumber { get; set; }

    /// <summary>
    /// Notes
    /// </summary>
    [MaxLength(500)]
    [Display(Name = "Notes")]
    [DataType(DataType.MultilineText)]
    public string? Notes { get; set; }

    /// <summary>
    /// Current stock level (for display)
    /// </summary>
    [Display(Name = "Current Stock")]
    public int CurrentStock { get; set; }

    /// <summary>
    /// Available products
    /// </summary>
    public SelectList Products { get; set; } = new(new List<SelectListItem>(), "Value", "Text");

    /// <summary>
    /// Available warehouses
    /// </summary>
    public SelectList Warehouses { get; set; } = new(new List<SelectListItem>(), "Value", "Text");
}

/// <summary>
/// ViewModel for creating adjustment transactions
/// </summary>
public class CreateAdjustmentViewModel
{
    /// <summary>
    /// Product ID
    /// </summary>
    [Required]
    [Display(Name = "Product")]
    public int ProductId { get; set; }

    /// <summary>
    /// Warehouse ID
    /// </summary>
    [Required]
    [Display(Name = "Warehouse")]
    public int WarehouseId { get; set; }

    /// <summary>
    /// Quantity adjustment
    /// </summary>
    [Required]
    [Display(Name = "Quantity Adjustment")]
    public int QuantityAdjustment { get; set; }

    /// <summary>
    /// Reason for adjustment
    /// </summary>
    [Required]
    [MaxLength(200)]
    [Display(Name = "Reason")]
    public string? Reason { get; set; }

    /// <summary>
    /// Reference number
    /// </summary>
    [MaxLength(50)]
    [Display(Name = "Reference Number")]
    public string? ReferenceNumber { get; set; }

    /// <summary>
    /// Notes
    /// </summary>
    [MaxLength(500)]
    [Display(Name = "Notes")]
    [DataType(DataType.MultilineText)]
    public string? Notes { get; set; }

    /// <summary>
    /// Current stock level (for display)
    /// </summary>
    [Display(Name = "Current Stock")]
    public int CurrentStock { get; set; }

    /// <summary>
    /// New stock level (calculated)
    /// </summary>
    [Display(Name = "New Stock Level")]
    public int NewStockLevel => CurrentStock + QuantityAdjustment;

    /// <summary>
    /// Available products
    /// </summary>
    public SelectList Products { get; set; } = new(new List<SelectListItem>(), "Value", "Text");

    /// <summary>
    /// Available warehouses
    /// </summary>
    public SelectList Warehouses { get; set; } = new(new List<SelectListItem>(), "Value", "Text");
}

/// <summary>
/// ViewModel for transaction details
/// </summary>
public class TransactionDetailsViewModel
{
    /// <summary>
    /// Transaction information
    /// </summary>
    public TransactionDto Transaction { get; set; } = new();

    /// <summary>
    /// Related transactions (same product/warehouse)
    /// </summary>
    public IEnumerable<TransactionDto> RelatedTransactions { get; set; } = new List<TransactionDto>();

    /// <summary>
    /// Current inventory level
    /// </summary>
    public InventoryDto? CurrentInventory { get; set; }
}

/// <summary>
/// ViewModel for transaction history
/// </summary>
public class TransactionHistoryViewModel
{
    /// <summary>
    /// Product information
    /// </summary>
    public ProductDto? Product { get; set; }

    /// <summary>
    /// Warehouse information
    /// </summary>
    public WarehouseDto? Warehouse { get; set; }

    /// <summary>
    /// Transaction history
    /// </summary>
    public IEnumerable<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();

    /// <summary>
    /// Transaction statistics
    /// </summary>
    public TransactionSummaryDto Statistics { get; set; } = new();

    /// <summary>
    /// Filter criteria
    /// </summary>
    public TransactionFilterViewModel Filter { get; set; } = new();

    /// <summary>
    /// Pagination information
    /// </summary>
    public PaginationViewModel Pagination { get; set; } = new();
}
