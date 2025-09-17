using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.Application.Features.Suppliers.Queries.GetSupplierWithProducts;

/// <summary>
/// Query to retrieve detailed supplier information with product analytics
/// </summary>
public class GetSupplierWithProductsQuery : IRequest<GetSupplierWithProductsQueryResponse>
{
    /// <summary>
    /// Supplier ID to retrieve
    /// </summary>
    public int SupplierId { get; set; }

    /// <summary>
    /// Filter for active products only
    /// </summary>
    public bool ActiveProductsOnly { get; set; } = false;

    /// <summary>
    /// Filter for low stock products only
    /// </summary>
    public bool LowStockProductsOnly { get; set; } = false;

    public GetSupplierWithProductsQuery(int supplierId, bool activeProductsOnly = false, bool lowStockProductsOnly = false)
    {
        SupplierId = supplierId;
        ActiveProductsOnly = activeProductsOnly;
        LowStockProductsOnly = lowStockProductsOnly;
    }
}

/// <summary>
/// Response for GetSupplierWithProductsQuery
/// </summary>
public class GetSupplierWithProductsQueryResponse
{
    /// <summary>
    /// Supplier information
    /// </summary>
    public SupplierDto? Supplier { get; set; }

    /// <summary>
    /// List of products from this supplier
    /// </summary>
    public IList<ProductDto> Products { get; set; } = new List<ProductDto>();

    /// <summary>
    /// Total number of products from this supplier
    /// </summary>
    public int TotalProducts { get; set; }

    /// <summary>
    /// Number of active products
    /// </summary>
    public int ActiveProducts { get; set; }

    /// <summary>
    /// Number of inactive products
    /// </summary>
    public int InactiveProducts { get; set; }

    /// <summary>
    /// Number of products with low stock
    /// </summary>
    public int LowStockProducts { get; set; }

    /// <summary>
    /// Total value of inventory from this supplier
    /// </summary>
    public decimal TotalInventoryValue { get; set; }

    /// <summary>
    /// Average product price from this supplier
    /// </summary>
    public decimal? AverageProductPrice { get; set; }

    /// <summary>
    /// Highest priced product from this supplier
    /// </summary>
    public decimal? HighestPrice { get; set; }

    /// <summary>
    /// Lowest priced product from this supplier
    /// </summary>
    public decimal? LowestPrice { get; set; }

    /// <summary>
    /// Whether the supplier was found
    /// </summary>
    public bool IsFound { get; set; }
}

/// <summary>
/// Handler for GetSupplierWithProductsQuery
/// </summary>
public class GetSupplierWithProductsQueryHandler : IRequestHandler<GetSupplierWithProductsQuery, GetSupplierWithProductsQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSupplierWithProductsQueryHandler> _logger;

    public GetSupplierWithProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSupplierWithProductsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetSupplierWithProductsQueryResponse> Handle(GetSupplierWithProductsQuery request, CancellationToken cancellationToken)
    {
        // Get supplier
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(request.SupplierId, cancellationToken: cancellationToken);

        if (supplier == null)
        {
            return new GetSupplierWithProductsQueryResponse
            {
                IsFound = false
            };
        }

        // Get products for this supplier
        var allSupplierProductsQuery = await _unitOfWork.Products.GetAsync(
            filter: p => p.SupplierId == request.SupplierId,
            includeProperties: "Category,InventoryItems",
            cancellationToken: cancellationToken);

        var allSupplierProducts = allSupplierProductsQuery.ToList();

        // Apply filters
        var filteredProducts = allSupplierProducts.AsEnumerable();
        
        if (request.ActiveProductsOnly)
        {
            filteredProducts = filteredProducts.Where(p => p.IsActive);
        }

        if (request.LowStockProductsOnly)
        {
            filteredProducts = filteredProducts.Where(p => 
                p.InventoryItems.Sum(i => i.Quantity) <= p.LowStockThreshold);
        }

        var products = filteredProducts.ToList();

        // Calculate statistics
        var totalProducts = allSupplierProducts.Count;
        var activeProducts = allSupplierProducts.Count(p => p.IsActive);
        var inactiveProducts = totalProducts - activeProducts;
        var lowStockProducts = allSupplierProducts.Count(p => 
            p.InventoryItems.Sum(i => i.Quantity) <= p.LowStockThreshold);

        var totalInventoryValue = allSupplierProducts.Sum(p => 
            p.InventoryItems.Sum(i => i.Quantity) * p.Price);

        var averagePrice = allSupplierProducts.Any() 
            ? allSupplierProducts.Average(p => p.Price) 
            : (decimal?)null;

        var highestPrice = allSupplierProducts.Any() 
            ? allSupplierProducts.Max(p => p.Price) 
            : (decimal?)null;

        var lowestPrice = allSupplierProducts.Any() 
            ? allSupplierProducts.Min(p => p.Price) 
            : (decimal?)null;

        // Map products to DTOs
        var productDtos = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            SKU = p.SKU,
            Description = p.Description,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name ?? string.Empty,
            SupplierId = p.SupplierId,
            SupplierName = supplier.Name,
            Price = p.Price,
            Cost = p.Cost,
            LowStockThreshold = p.LowStockThreshold,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            TotalQuantity = p.InventoryItems.Sum(i => i.Quantity),
            ProfitMarginPercentage = p.Cost.HasValue && p.Cost > 0 
                ? ((p.Price - p.Cost.Value) / p.Cost.Value) * 100 
                : (decimal?)null
        }).ToList();

        var supplierDto = new SupplierDto
        {
            Id = supplier.Id,
            Name = supplier.Name,
            ContactPerson = supplier.ContactInfo,
            Phone = supplier.Phone,
            Email = supplier.Email,
            Address = supplier.Address,
            IsActive = supplier.IsActive,
            CreatedAt = supplier.CreatedAt,
            UpdatedAt = supplier.UpdatedAt,
            ProductCount = totalProducts
        };

        return new GetSupplierWithProductsQueryResponse
        {
            Supplier = supplierDto,
            Products = productDtos,
            TotalProducts = totalProducts,
            ActiveProducts = activeProducts,
            InactiveProducts = inactiveProducts,
            LowStockProducts = lowStockProducts,
            TotalInventoryValue = totalInventoryValue,
            AverageProductPrice = averagePrice,
            HighestPrice = highestPrice,
            LowestPrice = lowestPrice,
            IsFound = true
        };
    }
}
