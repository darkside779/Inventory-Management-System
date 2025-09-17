using MediatR;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.Application.Features.Users.Commands.UpdateUser;

/// <summary>
/// Handler for UpdateUserCommand
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating user with ID: {UserId}", request.Id);

            // Get existing user
            var user = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {request.Id} not found.");
            }

            // Check if username is unique (excluding current user)
            if (user.Username != request.Username)
            {
                var isUsernameUnique = await _unitOfWork.Users.IsUsernameUniqueAsync(request.Username, request.Id, cancellationToken);
                if (!isUsernameUnique)
                {
                    throw new InvalidOperationException($"Username '{request.Username}' is already taken.");
                }
            }

            // Check if email is unique (excluding current user)
            if (user.Email != request.Email)
            {
                var isEmailUnique = await _unitOfWork.Users.IsEmailUniqueAsync(request.Email, request.Id, cancellationToken);
                if (!isEmailUnique)
                {
                    throw new InvalidOperationException($"Email '{request.Email}' is already registered.");
                }
            }

            // Update user properties
            user.Username = request.Username;
            user.Email = request.Email;
            user.FullName = request.FullName;
            user.PhoneNumber = request.PhoneNumber;
            user.Role = request.Role;
            user.IsActive = request.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            // Update user in repository
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User updated successfully with ID: {UserId}", user.Id);

            // Return UserDto
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

            return userDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID: {UserId}", request.Id);
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
