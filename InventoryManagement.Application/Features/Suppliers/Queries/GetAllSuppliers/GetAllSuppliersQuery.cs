using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Extensions;
using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.Application.Features.Suppliers.Queries.GetAllSuppliers;

/// <summary>
/// Query to retrieve all suppliers with pagination and filtering
/// </summary>
public class GetAllSuppliersQuery : IRequest<GetAllSuppliersQueryResponse>
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
    /// Search term to filter suppliers by name, contact person, or email
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter by active suppliers only
    /// </summary>
    public bool ActiveOnly { get; set; } = false;

    /// <summary>
    /// Filter by suppliers with products only
    /// </summary>
    public bool WithProductsOnly { get; set; } = false;

    /// <summary>
    /// Sort by field (Name, ContactPerson, Email, ProductCount, CreatedAt)
    /// </summary>
    public string SortBy { get; set; } = "Name";

    /// <summary>
    /// Sort direction (asc or desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";
}

/// <summary>
/// Response for GetAllSuppliersQuery
/// </summary>
public class GetAllSuppliersQueryResponse
{
    /// <summary>
    /// List of suppliers
    /// </summary>
    public IList<SupplierDto> Suppliers { get; set; } = new List<SupplierDto>();

    /// <summary>
    /// Total number of suppliers (before pagination)
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
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage { get; set; }
}

/// <summary>
/// Handler for GetAllSuppliersQuery
/// </summary>
public class GetAllSuppliersQueryHandler : IRequestHandler<GetAllSuppliersQuery, GetAllSuppliersQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllSuppliersQueryHandler> _logger;

    public GetAllSuppliersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllSuppliersQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetAllSuppliersQueryResponse> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving suppliers - Page: {PageNumber}, Size: {PageSize}, Search: {SearchTerm}", 
            request.PageNumber, request.PageSize, request.SearchTerm);

        // Apply filters and get paginated results
        var (suppliers, totalCount) = await _unitOfWork.Suppliers.GetPagedAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            filter: s => (!request.ActiveOnly || s.IsActive) &&
                        (!request.WithProductsOnly || s.Products.Any()) &&
                        (string.IsNullOrEmpty(request.SearchTerm) || 
                         s.Name.Contains(request.SearchTerm) || 
                         (s.Email != null && s.Email.Contains(request.SearchTerm)) ||
                         (s.Phone != null && s.Phone.Contains(request.SearchTerm))),
            orderBy: request.SortBy.ToLower() switch
            {
                "name" => request.SortDirection == "desc" ? q => q.OrderByDescending(s => s.Name) : q => q.OrderBy(s => s.Name),
                "email" => request.SortDirection == "desc" ? q => q.OrderByDescending(s => s.Email) : q => q.OrderBy(s => s.Email),
                "productcount" => request.SortDirection == "desc" ? q => q.OrderByDescending(s => s.Products.Count) : q => q.OrderBy(s => s.Products.Count),
                "createdat" => request.SortDirection == "desc" ? q => q.OrderByDescending(s => s.CreatedAt) : q => q.OrderBy(s => s.CreatedAt),
                _ => q => q.OrderBy(s => s.Name)
            },
            includeProperties: "Products",
            cancellationToken: cancellationToken);

        var supplierDtos = _mapper.Map<IEnumerable<SupplierDto>>(suppliers);

        var result = new GetAllSuppliersQueryResponse
        {
            Suppliers = supplierDtos.ToList(),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
            HasNextPage = request.PageNumber < (int)Math.Ceiling((double)totalCount / request.PageSize),
            HasPreviousPage = request.PageNumber > 1
        };

        _logger.LogInformation("Retrieved {SupplierCount} suppliers out of {TotalCount} total", 
            supplierDtos.Count(), totalCount);

        return result;
    }
}
