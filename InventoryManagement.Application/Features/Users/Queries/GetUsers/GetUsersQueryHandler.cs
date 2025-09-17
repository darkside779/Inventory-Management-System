using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;
using System.Linq.Expressions;

namespace InventoryManagement.Application.Features.Users.Queries.GetUsers;

/// <summary>
/// Handler for GetUsersQuery
/// </summary>
public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, GetUsersQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetUsersQueryHandler> _logger;

    public GetUsersQueryHandler(IUnitOfWork unitOfWork, ILogger<GetUsersQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<GetUsersQueryResponse> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting users with filters - SearchTerm: {SearchTerm}, Role: {Role}, IsActive: {IsActive}, Page: {Page}, PageSize: {PageSize}",
                request.SearchTerm, request.Role, request.IsActive, request.Page, request.PageSize);

            // Build filter expression
            Expression<Func<User, bool>>? filter = null;
            
            if (!string.IsNullOrWhiteSpace(request.SearchTerm) || request.Role.HasValue || 
                request.IsActive.HasValue || request.CreatedFrom.HasValue || 
                request.CreatedTo.HasValue || request.HasRecentLogin == true)
            {
                filter = u => 
                    (string.IsNullOrWhiteSpace(request.SearchTerm) || 
                     u.FullName.ToLower().Contains(request.SearchTerm.ToLower()) ||
                     u.Username.ToLower().Contains(request.SearchTerm.ToLower()) ||
                     u.Email.ToLower().Contains(request.SearchTerm.ToLower())) &&
                    (!request.Role.HasValue || u.Role == request.Role.Value) &&
                    (!request.IsActive.HasValue || u.IsActive == request.IsActive.Value) &&
                    (!request.CreatedFrom.HasValue || u.CreatedAt >= request.CreatedFrom.Value) &&
                    (!request.CreatedTo.HasValue || u.CreatedAt < request.CreatedTo.Value.AddDays(1)) &&
                    (request.HasRecentLogin != true || (u.LastLoginAt.HasValue && u.LastLoginAt >= DateTime.UtcNow.AddDays(-30)));
            }

            // Build ordering function
            Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null;
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                orderBy = query => ApplySorting(query, request.SortBy, request.SortDirection);
            }

            // Get paginated results
            var (users, totalCount) = await _unitOfWork.Users.GetPagedAsync(
                request.Page, 
                request.PageSize, 
                filter, 
                orderBy, 
                "", 
                cancellationToken);

            // Convert to DTOs
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FirstName = GetFirstName(u.FullName),
                LastName = GetLastName(u.FullName),
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                LastLoginAt = u.LastLoginAt
            }).ToList();

            var response = new GetUsersQueryResponse
            {
                Users = userDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            _logger.LogInformation("Retrieved {Count} users out of {Total} total users", userDtos.Count, totalCount);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users with filters");
            throw;
        }
    }

    /// <summary>
    /// Apply sorting to the query
    /// </summary>
    private IOrderedQueryable<User> ApplySorting(IQueryable<User> query, string sortBy, string sortDirection)
    {
        var isDescending = sortDirection.ToLower() == "desc";

        return sortBy.ToLower() switch
        {
            "username" => isDescending ? query.OrderByDescending(u => u.Username) : query.OrderBy(u => u.Username),
            "email" => isDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "role" => isDescending ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role),
            "isactive" => isDescending ? query.OrderByDescending(u => u.IsActive) : query.OrderBy(u => u.IsActive),
            "createdat" => isDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
            "lastloginat" => isDescending ? query.OrderByDescending(u => u.LastLoginAt) : query.OrderBy(u => u.LastLoginAt),
            _ => isDescending ? query.OrderByDescending(u => u.FullName) : query.OrderBy(u => u.FullName)
        };
    }

    /// <summary>
    /// Extract first name from full name
    /// </summary>
    private string GetFirstName(string fullName)
    {
        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0] : string.Empty;
    }

    /// <summary>
    /// Extract last name from full name
    /// </summary>
    private string GetLastName(string fullName)
    {
        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : string.Empty;
    }
}
