using MediatR;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.Application.Features.Users.Queries.GetUserById;

/// <summary>
/// Handler for GetUserByIdQuery
/// </summary>
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetUserByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting user by ID: {UserId}", request.Id);

            var user = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", request.Id);
                return null;
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = GetFirstName(user.FullName),
                LastName = GetLastName(user.FullName),
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                LastLoginAt = user.LastLoginAt
            };

            _logger.LogInformation("Successfully retrieved user with ID: {UserId}", request.Id);
            return userDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {UserId}", request.Id);
            throw;
        }
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
