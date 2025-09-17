using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces;

/// <summary>
/// Repository interface for Product entity with specific business operations
/// </summary>
public interface IProductRepository : IGenericRepository<Product>
{
    /// <summary>
    /// Get products by category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Products in the category</returns>
    Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get products by supplier
    /// </summary>
    /// <param name="supplierId">Supplier ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Products from the supplier</returns>
    Task<IEnumerable<Product>> GetBySupplierAsync(int supplierId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search products by name or SKU
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Matching products</returns>
    Task<IEnumerable<Product>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get product by SKU
    /// </summary>
    /// <param name="sku">Product SKU</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Product or null</returns>
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get product by barcode
    /// </summary>
    /// <param name="barcode">Product barcode</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Product or null</returns>
    Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get products with low stock across all warehouses
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Products with low stock</returns>
    Task<IEnumerable<Product>> GetLowStockProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get products with their total inventory quantity
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Products with total quantities</returns>
    Task<IEnumerable<(Product Product, int TotalQuantity)>> GetProductsWithTotalQuantityAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if SKU is unique
    /// </summary>
    /// <param name="sku">SKU to check</param>
    /// <param name="excludeProductId">Product ID to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if SKU is unique</returns>
    Task<bool> IsSkuUniqueAsync(string sku, int? excludeProductId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if barcode is unique
    /// </summary>
    /// <param name="barcode">Barcode to check</param>
    /// <param name="excludeProductId">Product ID to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if barcode is unique</returns>
    Task<bool> IsBarcodeUniqueAsync(string barcode, int? excludeProductId = null, CancellationToken cancellationToken = default);
}
