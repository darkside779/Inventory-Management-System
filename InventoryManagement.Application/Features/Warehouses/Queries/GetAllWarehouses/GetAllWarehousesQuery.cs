using MediatR;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Features.Warehouses.Queries.GetAllWarehouses;

/// <summary>
/// Query to retrieve paginated list of warehouses with filtering options
/// </summary>
public class GetAllWarehousesQuery : IRequest<GetAllWarehousesQueryResponse>
{
    /// <summary>
    /// Page number for pagination (starts from 1)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Search term to filter warehouses by name or location
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter to show only active warehouses
    /// </summary>
    public bool ActiveOnly { get; set; } = true;

    /// <summary>
    /// Sort field
    /// </summary>
    public string SortBy { get; set; } = "Name";

    /// <summary>
    /// Sort direction (asc or desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";

    /// <summary>
    /// Filter by capacity range - minimum capacity
    /// </summary>
    public int? MinCapacity { get; set; }

    /// <summary>
    /// Filter by capacity range - maximum capacity
    /// </summary>
    public int? MaxCapacity { get; set; }
}

/// <summary>
/// Response for GetAllWarehousesQuery
/// </summary>
public class GetAllWarehousesQueryResponse
{
    /// <summary>
    /// List of warehouses for the current page
    /// </summary>
    public List<WarehouseDto> Warehouses { get; set; } = new();

    /// <summary>
    /// Total number of warehouses matching the criteria
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
    /// Indicates if there are more pages after current page
    /// </summary>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// Indicates if there are pages before current page
    /// </summary>
    public bool HasPreviousPage { get; set; }
}

/// <summary>
/// Handler for GetAllWarehousesQuery
/// </summary>
public class GetAllWarehousesQueryHandler : IRequestHandler<GetAllWarehousesQuery, GetAllWarehousesQueryResponse>
{
    private readonly IApplicationDbContext _context;

    public GetAllWarehousesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetAllWarehousesQueryResponse> Handle(GetAllWarehousesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Warehouses
            .Include(w => w.InventoryItems)
            .AsQueryable();

        // Apply filters
        if (request.ActiveOnly)
        {
            query = query.Where(w => w.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(w => 
                w.Name.ToLower().Contains(searchTerm) ||
                w.Location.ToLower().Contains(searchTerm) ||
                (w.Address != null && w.Address.ToLower().Contains(searchTerm)));
        }

        if (request.MinCapacity.HasValue)
        {
            query = query.Where(w => w.Capacity >= request.MinCapacity.Value);
        }

        if (request.MaxCapacity.HasValue)
        {
            query = query.Where(w => w.Capacity <= request.MaxCapacity.Value);
        }

        // Apply sorting
        query = request.SortBy.ToLower() switch
        {
            "location" => request.SortDirection.ToLower() == "desc" 
                ? query.OrderByDescending(w => w.Location)
                : query.OrderBy(w => w.Location),
            "capacity" => request.SortDirection.ToLower() == "desc"
                ? query.OrderByDescending(w => w.Capacity)
                : query.OrderBy(w => w.Capacity),
            "inventorycount" => request.SortDirection.ToLower() == "desc"
                ? query.OrderByDescending(w => w.InventoryItems.Count)
                : query.OrderBy(w => w.InventoryItems.Count),
            "createdat" => request.SortDirection.ToLower() == "desc"
                ? query.OrderByDescending(w => w.CreatedAt)
                : query.OrderBy(w => w.CreatedAt),
            "updatedat" => request.SortDirection.ToLower() == "desc"
                ? query.OrderByDescending(w => w.UpdatedAt)
                : query.OrderBy(w => w.UpdatedAt),
            _ => request.SortDirection.ToLower() == "desc"
                ? query.OrderByDescending(w => w.Name)
                : query.OrderBy(w => w.Name)
        };

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var warehouses = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(w => new WarehouseDto
            {
                Id = w.Id,
                Name = w.Name,
                Location = w.Location,
                Description = w.Address,
                Capacity = w.Capacity,
                IsActive = w.IsActive,
                CreatedAt = w.CreatedAt,
                UpdatedAt = w.UpdatedAt,
                InventoryItemCount = w.InventoryItems.Count,
                CapacityUtilization = w.Capacity.HasValue && w.Capacity.Value > 0 
                    ? (decimal)w.InventoryItems.Sum(i => i.Quantity) / w.Capacity.Value * 100 
                    : null
            })
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new GetAllWarehousesQueryResponse
        {
            Warehouses = warehouses,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = totalPages,
            HasNextPage = request.PageNumber < totalPages,
            HasPreviousPage = request.PageNumber > 1
        };
    }
}
