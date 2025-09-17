using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// Product repository implementation with specific business operations
/// </summary>
public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get products by category
    /// </summary>
    public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get products by supplier
    /// </summary>
    public async Task<IEnumerable<Product>> GetBySupplierAsync(int supplierId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.SupplierId == supplierId && p.IsActive)
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Search products by name or SKU
    /// </summary>
    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        
        return await _dbSet
            .Where(p => p.IsActive && (
                p.Name.ToLower().Contains(lowerSearchTerm) ||
                p.SKU.ToLower().Contains(lowerSearchTerm) ||
                (p.Description != null && p.Description.ToLower().Contains(lowerSearchTerm))
            ))
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get product by SKU
    /// </summary>
    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.SKU == sku, cancellationToken);
    }

    /// <summary>
    /// Get product by barcode
    /// </summary>
    public async Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.Barcode == barcode, cancellationToken);
    }

    /// <summary>
    /// Get products with low stock across all warehouses
    /// </summary>
    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.IsActive)
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Include(p => p.InventoryItems)
            .Where(p => p.InventoryItems.Sum(i => i.Quantity) <= p.LowStockThreshold)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get products with their total inventory quantity
    /// </summary>
    public async Task<IEnumerable<(Product Product, int TotalQuantity)>> GetProductsWithTotalQuantityAsync(CancellationToken cancellationToken = default)
    {
        var query = from p in _dbSet
                    where p.IsActive
                    select new
                    {
                        Product = p,
                        TotalQuantity = p.InventoryItems.Sum(i => i.Quantity)
                    };

        var results = await query
            .Include(x => x.Product.Category)
            .Include(x => x.Product.Supplier)
            .OrderBy(x => x.Product.Name)
            .ToListAsync(cancellationToken);

        return results.Select(r => (r.Product, r.TotalQuantity));
    }

    /// <summary>
    /// Check if SKU is unique
    /// </summary>
    public async Task<bool> IsSkuUniqueAsync(string sku, int? excludeProductId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(p => p.SKU == sku);
        
        if (excludeProductId.HasValue)
        {
            query = query.Where(p => p.Id != excludeProductId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Check if barcode is unique
    /// </summary>
    public async Task<bool> IsBarcodeUniqueAsync(string barcode, int? excludeProductId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(barcode))
        {
            return true; // Allow null/empty barcodes
        }

        var query = _dbSet.Where(p => p.Barcode == barcode);
        
        if (excludeProductId.HasValue)
        {
            query = query.Where(p => p.Id != excludeProductId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }
}
