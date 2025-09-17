using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs;

/// <summary>
/// Data Transfer Object for Product entity
/// </summary>
public class ProductDto
{
    /// <summary>
    /// Unique identifier for the product
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Name of the product
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Stock Keeping Unit - unique identifier for the product
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string SKU { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed description of the product
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Selling price of the product
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
    public decimal Price { get; set; }
    
    /// <summary>
    /// Cost price of the product
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Cost cannot be negative")]
    public decimal? Cost { get; set; }
    
    /// <summary>
    /// Category ID (foreign key)
    /// </summary>
    [Required]
    public int CategoryId { get; set; }
    
    /// <summary>
    /// Category name
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;
    
    /// <summary>
    /// Supplier ID (foreign key) - optional
    /// </summary>
    public int? SupplierId { get; set; }
    
    /// <summary>
    /// Supplier name
    /// </summary>
    public string? SupplierName { get; set; }
    
    /// <summary>
    /// Low stock alert threshold
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Low stock threshold cannot be negative")]
    public int LowStockThreshold { get; set; } = 10;
    
    /// <summary>
    /// Unit of measurement (e.g., "Piece", "Kg", "Liter")
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Unit { get; set; } = "Piece";
    
    /// <summary>
    /// Product barcode
    /// </summary>
    [MaxLength(100)]
    public string? Barcode { get; set; }
    
    /// <summary>
    /// Product weight
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Weight cannot be negative")]
    public decimal? Weight { get; set; }
    
    /// <summary>
    /// Product dimensions (length x width x height)
    /// </summary>
    [MaxLength(100)]
    public string? Dimensions { get; set; }
    
    /// <summary>
    /// Indicates whether the product is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Date and time when the product was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Date and time when the product was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Total quantity across all warehouses
    /// </summary>
    public int TotalQuantity { get; set; }
    
    /// <summary>
    /// Indicates if the product is low on stock
    /// </summary>
    public bool IsLowStock => TotalQuantity <= LowStockThreshold;
    
    /// <summary>
    /// Profit margin percentage
    /// </summary>
    public decimal? ProfitMarginPercentage { get; set; }
}

/// <summary>
/// DTO for creating a new product
/// </summary>
public class CreateProductDto
{
    /// <summary>
    /// Name of the product
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Stock Keeping Unit - unique identifier for the product
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string SKU { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed description of the product
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Selling price of the product
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
    public decimal Price { get; set; }
    
    /// <summary>
    /// Cost price of the product
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Cost cannot be negative")]
    public decimal? Cost { get; set; }
    
    /// <summary>
    /// Category ID (foreign key)
    /// </summary>
    [Required]
    public int CategoryId { get; set; }
    
    /// <summary>
    /// Supplier ID (foreign key) - optional
    /// </summary>
    public int? SupplierId { get; set; }
    
    /// <summary>
    /// Low stock alert threshold
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Low stock threshold cannot be negative")]
    public int LowStockThreshold { get; set; } = 10;
    
    /// <summary>
    /// Unit of measurement (e.g., "Piece", "Kg", "Liter")
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Unit { get; set; } = "Piece";
    
    /// <summary>
    /// Product barcode
    /// </summary>
    [MaxLength(100)]
    public string? Barcode { get; set; }
    
    /// <summary>
    /// Product weight
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Weight cannot be negative")]
    public decimal? Weight { get; set; }
    
    /// <summary>
    /// Product dimensions (length x width x height)
    /// </summary>
    [MaxLength(100)]
    public string? Dimensions { get; set; }
    
    /// <summary>
    /// Indicates whether the product is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO for updating an existing product
/// </summary>
public class UpdateProductDto
{
    /// <summary>
    /// Unique identifier for the product
    /// </summary>
    [Required]
    public int Id { get; set; }
    
    /// <summary>
    /// Name of the product
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Stock Keeping Unit - unique identifier for the product
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string SKU { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed description of the product
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Selling price of the product
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
    public decimal Price { get; set; }
    
    /// <summary>
    /// Cost price of the product
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Cost cannot be negative")]
    public decimal? Cost { get; set; }
    
    /// <summary>
    /// Category ID (foreign key)
    /// </summary>
    [Required]
    public int CategoryId { get; set; }
    
    /// <summary>
    /// Supplier ID (foreign key) - optional
    /// </summary>
    public int? SupplierId { get; set; }
    
    /// <summary>
    /// Low stock alert threshold
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Low stock threshold cannot be negative")]
    public int LowStockThreshold { get; set; }
    
    /// <summary>
    /// Unit of measurement (e.g., "Piece", "Kg", "Liter")
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Unit { get; set; } = "Piece";
    
    /// <summary>
    /// Product barcode
    /// </summary>
    [MaxLength(100)]
    public string? Barcode { get; set; }
    
    /// <summary>
    /// Product weight
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Weight cannot be negative")]
    public decimal? Weight { get; set; }
    
    /// <summary>
    /// Product dimensions (length x width x height)
    /// </summary>
    [MaxLength(100)]
    public string? Dimensions { get; set; }
    
    /// <summary>
    /// Indicates whether the product is active
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// Simplified DTO for product lookups and dropdowns
/// </summary>
public class ProductLookupDto
{
    /// <summary>
    /// Unique identifier for the product
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Name of the product
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Stock Keeping Unit
    /// </summary>
    public string SKU { get; set; } = string.Empty;
    
    /// <summary>
    /// Selling price of the product
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Unit of measurement
    /// </summary>
    public string Unit { get; set; } = string.Empty;
    
    /// <summary>
    /// Indicates whether the product is active
    /// </summary>
    public bool IsActive { get; set; }
}
