using MediatR;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Features.Inventory.Queries.GetAllInventory;

/// <summary>
/// Query to retrieve paginated list of inventory records with filtering options
/// </summary>
public class GetAllInventoryQuery : IRequest<GetAllInventoryQueryResponse>
{
    /// <summary>
    /// Page number for pagination (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;
    
    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; } = 10;
    
    /// <summary>
    /// Search term for product name or SKU
    /// </summary>
    public string? SearchTerm { get; set; }
    
    /// <summary>
    /// Filter by specific warehouse ID
    /// </summary>
    public int? WarehouseId { get; set; }
    
    /// <summary>
    /// Filter by low stock items only
    /// </summary>
    public bool? LowStockOnly { get; set; }
    
    /// <summary>
    /// Filter by specific product ID
    /// </summary>
    public int? ProductId { get; set; }
    
    /// <summary>
    /// Sort field (ProductName, WarehouseName, Quantity, AvailableQuantity)
    /// </summary>
    public string SortBy { get; set; } = "ProductName";
    
    /// <summary>
    /// Sort direction (asc, desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";
    
    /// <summary>
    /// Include only active inventory records
    /// </summary>
    public bool ActiveOnly { get; set; } = true;
}

/// <summary>
/// Response for GetAllInventoryQuery
/// </summary>
public class GetAllInventoryQueryResponse
{
    /// <summary>
    /// List of inventory records
    /// </summary>
    public IEnumerable<InventoryDto> Inventories { get; set; } = new List<InventoryDto>();
    
    /// <summary>
    /// Total number of records (before pagination)
    /// </summary>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// Current page number
    /// </summary>
    public int PageNumber { get; set; }
    
    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }
    
    /// <summary>
    /// Indicates if there are more pages
    /// </summary>
    public bool HasNextPage { get; set; }
    
    /// <summary>
    /// Indicates if there are previous pages
    /// </summary>
    public bool HasPreviousPage { get; set; }
}

/// <summary>
/// Handler for GetAllInventoryQuery
/// </summary>
public class GetAllInventoryQueryHandler : IRequestHandler<GetAllInventoryQuery, GetAllInventoryQueryResponse>
{
    private readonly IApplicationDbContext _context;

    public GetAllInventoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetAllInventoryQueryResponse> Handle(GetAllInventoryQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Inventories
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .AsQueryable();

        // Apply filters
        if (request.ActiveOnly)
        {
            query = query.Where(i => i.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(i => 
                i.Product.Name.ToLower().Contains(searchTerm) ||
                i.Product.SKU.ToLower().Contains(searchTerm) ||
                i.Warehouse.Name.ToLower().Contains(searchTerm));
        }

        if (request.WarehouseId.HasValue)
        {
            query = query.Where(i => i.WarehouseId == request.WarehouseId.Value);
        }

        if (request.ProductId.HasValue)
        {
            query = query.Where(i => i.ProductId == request.ProductId.Value);
        }

        if (request.LowStockOnly.HasValue && request.LowStockOnly.Value)
        {
            query = query.Where(i => i.Quantity <= i.Product.LowStockThreshold);
        }

        // Apply sorting
        query = request.SortBy.ToLower() switch
        {
            "warehousename" => request.SortDirection.ToLower() == "desc" 
                ? query.OrderByDescending(i => i.Warehouse.Name)
                : query.OrderBy(i => i.Warehouse.Name),
            "quantity" => request.SortDirection.ToLower() == "desc"
                ? query.OrderByDescending(i => i.Quantity)
                : query.OrderBy(i => i.Quantity),
            "availablequantity" => request.SortDirection.ToLower() == "desc"
                ? query.OrderByDescending(i => i.Quantity - i.ReservedQuantity)
                : query.OrderBy(i => i.Quantity - i.ReservedQuantity),
            "updatedat" => request.SortDirection.ToLower() == "desc"
                ? query.OrderByDescending(i => i.UpdatedAt)
                : query.OrderBy(i => i.UpdatedAt),
            _ => request.SortDirection.ToLower() == "desc"
                ? query.OrderByDescending(i => i.Product.Name)
                : query.OrderBy(i => i.Product.Name)
        };

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var inventories = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(i => new InventoryDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                ProductSKU = i.Product.SKU,
                WarehouseId = i.WarehouseId,
                WarehouseName = i.Warehouse.Name,
                Quantity = i.Quantity,
                ReservedQuantity = i.ReservedQuantity,
                MinimumStockLevel = i.Product.LowStockThreshold,
                MaximumStockLevel = 1000, // Default or from business rules
                Location = i.Warehouse.Location,
                IsActive = i.IsActive,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
                ProductPrice = i.Product.Price
            })
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new GetAllInventoryQueryResponse
        {
            Inventories = inventories,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = totalPages,
            HasNextPage = request.PageNumber < totalPages,
            HasPreviousPage = request.PageNumber > 1
        };
    }
}
