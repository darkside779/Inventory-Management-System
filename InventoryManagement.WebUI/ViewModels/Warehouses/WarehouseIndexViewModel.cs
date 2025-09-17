using System.ComponentModel.DataAnnotations;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.WebUI.ViewModels.Warehouses;

/// <summary>
/// ViewModel for warehouse listing page
/// </summary>
public class WarehouseIndexViewModel
{
    /// <summary>
    /// List of warehouses for current page
    /// </summary>
    public List<WarehouseDto> Warehouses { get; set; } = new();
    
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
    /// Filter by active warehouses only
    /// </summary>
    public bool ActiveOnly { get; set; } = true;
    
    /// <summary>
    /// Sort field
    /// </summary>
    public string SortBy { get; set; } = "Name";
    
    /// <summary>
    /// Sort direction
    /// </summary>
    public string SortDirection { get; set; } = "asc";
    
    /// <summary>
    /// Filter by minimum capacity
    /// </summary>
    public int? MinCapacity { get; set; }
    
    /// <summary>
    /// Filter by maximum capacity
    /// </summary>
    public int? MaxCapacity { get; set; }
    
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
    public string PageTitle { get; set; } = "Warehouse Management";
}

/// <summary>
/// ViewModel for warehouse details page
/// </summary>
public class WarehouseDetailsViewModel
{
    /// <summary>
    /// Warehouse ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Warehouse name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Warehouse location
    /// </summary>
    public string Location { get; set; } = string.Empty;
    
    /// <summary>
    /// Full address
    /// </summary>
    public string? Address { get; set; }
    
    /// <summary>
    /// Contact phone
    /// </summary>
    public string? ContactPhone { get; set; }
    
    /// <summary>
    /// Contact email
    /// </summary>
    public string? ContactEmail { get; set; }
    
    /// <summary>
    /// Storage capacity
    /// </summary>
    public int? Capacity { get; set; }
    
    /// <summary>
    /// Is warehouse active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Created date
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Last updated date
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Inventory items in this warehouse
    /// </summary>
    public List<InventoryDto> InventoryItems { get; set; } = new();
    
    /// <summary>
    /// Total number of different products
    /// </summary>
    public int TotalProducts { get; set; }
    
    /// <summary>
    /// Total quantity of all items
    /// </summary>
    public int TotalQuantity { get; set; }
    
    /// <summary>
    /// Total value of all inventory
    /// </summary>
    public decimal TotalValue { get; set; }
    
    /// <summary>
    /// Number of low stock items
    /// </summary>
    public int LowStockItemsCount { get; set; }
    
    /// <summary>
    /// Current utilization percentage
    /// </summary>
    public decimal? UtilizationPercentage { get; set; }
    
    /// <summary>
    /// Page title
    /// </summary>
    public string PageTitle => $"Warehouse Details - {Name}";
    
    /// <summary>
    /// Capacity status for UI display
    /// </summary>
    public string CapacityStatus
    {
        get
        {
            if (!Capacity.HasValue || !UtilizationPercentage.HasValue) return "Unknown";
            
            return UtilizationPercentage.Value switch
            {
                < 50 => "Low",
                < 80 => "Medium",
                < 95 => "High",
                _ => "Critical"
            };
        }
    }
    
    /// <summary>
    /// CSS class for capacity status
    /// </summary>
    public string CapacityStatusClass
    {
        get
        {
            return CapacityStatus switch
            {
                "Low" => "text-success",
                "Medium" => "text-warning",
                "High" => "text-danger",
                "Critical" => "text-danger fw-bold",
                _ => "text-muted"
            };
        }
    }
}

/// <summary>
/// ViewModel for creating new warehouse
/// </summary>
public class CreateWarehouseViewModel
{
    /// <summary>
    /// Warehouse name
    /// </summary>
    [Required(ErrorMessage = "Warehouse name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    [Display(Name = "Warehouse Name")]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Warehouse location
    /// </summary>
    [Required(ErrorMessage = "Location is required")]
    [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
    [Display(Name = "Location")]
    public string Location { get; set; } = string.Empty;
    
    /// <summary>
    /// Full address
    /// </summary>
    [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    [Display(Name = "Full Address")]
    public string? Address { get; set; }
    
    /// <summary>
    /// Contact phone
    /// </summary>
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    [Display(Name = "Contact Phone")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string? ContactPhone { get; set; }
    
    /// <summary>
    /// Contact email
    /// </summary>
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    [Display(Name = "Contact Email")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    public string? ContactEmail { get; set; }
    
    /// <summary>
    /// Storage capacity
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
    [Display(Name = "Storage Capacity")]
    public int? Capacity { get; set; }
    
    /// <summary>
    /// Is warehouse active
    /// </summary>
    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Page title
    /// </summary>
    public string PageTitle => "Create New Warehouse";
}

/// <summary>
/// ViewModel for editing existing warehouse
/// </summary>
public class EditWarehouseViewModel
{
    /// <summary>
    /// Warehouse ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Warehouse name
    /// </summary>
    [Required(ErrorMessage = "Warehouse name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    [Display(Name = "Warehouse Name")]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Warehouse location
    /// </summary>
    [Required(ErrorMessage = "Location is required")]
    [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
    [Display(Name = "Location")]
    public string Location { get; set; } = string.Empty;
    
    /// <summary>
    /// Full address
    /// </summary>
    [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    [Display(Name = "Full Address")]
    public string? Address { get; set; }
    
    /// <summary>
    /// Contact phone
    /// </summary>
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    [Display(Name = "Contact Phone")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string? ContactPhone { get; set; }
    
    /// <summary>
    /// Contact email
    /// </summary>
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    [Display(Name = "Contact Email")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    public string? ContactEmail { get; set; }
    
    /// <summary>
    /// Storage capacity
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
    [Display(Name = "Storage Capacity")]
    public int? Capacity { get; set; }
    
    /// <summary>
    /// Is warehouse active
    /// </summary>
    [Display(Name = "Active")]
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Page title
    /// </summary>
    public string PageTitle => "Edit Warehouse";
}

/// <summary>
/// ViewModel for warehouse deletion confirmation
/// </summary>
public class DeleteWarehouseViewModel
{
    /// <summary>
    /// Warehouse ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Warehouse name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Warehouse location
    /// </summary>
    public string Location { get; set; } = string.Empty;
    
    /// <summary>
    /// Full address
    /// </summary>
    public string? Address { get; set; }
    
    /// <summary>
    /// Storage capacity
    /// </summary>
    public int? Capacity { get; set; }
    
    /// <summary>
    /// Is warehouse active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Created date
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Last updated date
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Total number of products
    /// </summary>
    public int TotalProducts { get; set; }
    
    /// <summary>
    /// Total quantity
    /// </summary>
    public int TotalQuantity { get; set; }
    
    /// <summary>
    /// Total inventory value
    /// </summary>
    public decimal TotalValue { get; set; }
    
    /// <summary>
    /// Has active inventory items
    /// </summary>
    public bool HasActiveInventory { get; set; }
    
    /// <summary>
    /// Has recent transactions
    /// </summary>
    public bool HasRecentTransactions { get; set; }
    
    /// <summary>
    /// Total inventory value (alias for TotalValue)
    /// </summary>
    public decimal TotalInventoryValue => TotalValue;
    
    /// <summary>
    /// Page title
    /// </summary>
    public string PageTitle => $"Delete Warehouse - {Name}";
    
    /// <summary>
    /// Can be deleted safely
    /// </summary>
    public bool CanDelete => !HasActiveInventory;
    
    /// <summary>
    /// Warning message for deletion
    /// </summary>
    public string DeletionWarning
    {
        get
        {
            if (HasActiveInventory)
                return "This warehouse contains active inventory items and cannot be deleted. Please transfer all inventory to other warehouses first.";
            
            if (TotalProducts > 0)
                return "This warehouse has inventory history. Deletion will deactivate the warehouse but preserve historical data.";
            
            return "This warehouse will be permanently deactivated.";
        }
    }
}
