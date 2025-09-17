using MediatR;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Application.Features.Users.Queries.GetUsers;

/// <summary>
/// Query to get users with filtering and pagination
/// </summary>
public class GetUsersQuery : IRequest<GetUsersQueryResponse>
{
    /// <summary>
    /// Search term for name, username, or email
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter by role
    /// </summary>
    public UserRole? Role { get; set; }

    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Date range start for user creation
    /// </summary>
    public DateTime? CreatedFrom { get; set; }

    /// <summary>
    /// Date range end for user creation
    /// </summary>
    public DateTime? CreatedTo { get; set; }

    /// <summary>
    /// Show only users who have logged in recently (last 30 days)
    /// </summary>
    public bool? HasRecentLogin { get; set; }

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 25;

    /// <summary>
    /// Sort field
    /// </summary>
    public string SortBy { get; set; } = "FullName";

    /// <summary>
    /// Sort direction (asc/desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";
}

/// <summary>
/// Response for GetUsersQuery
/// </summary>
public class GetUsersQueryResponse
{
    /// <summary>
    /// Users in current page
    /// </summary>
    public IEnumerable<UserDto> Users { get; set; } = new List<UserDto>();

    /// <summary>
    /// Total number of users matching criteria
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => Page < TotalPages;
}
