using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Application.Extensions;

namespace InventoryManagement.Application.Features.Products.Queries.GetAllProducts;

/// <summary>
/// Query to get all products with pagination
/// </summary>
public record GetAllProductsQuery : IRequest<PagedResult<ProductDto>>
{
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Items per page
    /// </summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Search term for product name or SKU
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Filter by category ID
    /// </summary>
    public int? CategoryId { get; init; }

    /// <summary>
    /// Include only active products
    /// </summary>
    public bool ActiveOnly { get; init; } = true;
}

/// <summary>
/// Handler for GetAllProductsQuery
/// </summary>
public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PagedResult<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllProductsQueryHandler> _logger;

    public GetAllProductsQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllProductsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving products - Page: {PageNumber}, Size: {PageSize}, Search: {SearchTerm}", 
            request.PageNumber, request.PageSize, request.SearchTerm);

        // Apply filters and get paginated results
        var (products, totalCount) = await _unitOfWork.Products.GetPagedAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            filter: p => (!request.ActiveOnly || p.IsActive) &&
                        (!request.CategoryId.HasValue || p.CategoryId == request.CategoryId.Value) &&
                        (string.IsNullOrEmpty(request.SearchTerm) || 
                         p.Name.Contains(request.SearchTerm) || 
                         p.SKU.Contains(request.SearchTerm)),
            orderBy: q => q.OrderBy(p => p.Name),
            includeProperties: "Category,Supplier",
            cancellationToken: cancellationToken);

        var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

        var result = new PagedResult<ProductDto>
        {
            Items = productDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        _logger.LogInformation("Retrieved {ProductCount} products out of {TotalCount} total", 
            productDtos.Count(), totalCount);

        return result;
    }
}
