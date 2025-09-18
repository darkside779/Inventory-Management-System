using MediatR;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace InventoryManagement.Application.Features.Users.Commands.CreateUser;

/// <summary>
/// Handler for CreateUserCommand
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateUserCommandHandler> _logger;
    private readonly IIdentityService _identityService;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateUserCommandHandler> logger, IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _identityService = identityService;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating new user with username: {Username}", request.Username);

            // Check if username already exists in custom Users table
            var existingUserByUsername = await _unitOfWork.Users.GetByUsernameAsync(request.Username, cancellationToken);
            if (existingUserByUsername != null)
            {
                throw new InvalidOperationException($"Username '{request.Username}' is already taken.");
            }

            // Check if email already exists in custom Users table
            var existingUserByEmail = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUserByEmail != null)
            {
                throw new InvalidOperationException($"Email '{request.Email}' is already registered.");
            }

            // Check if email already exists in Identity system
            var identityUserExists = await _identityService.UserExistsByEmailAsync(request.Email, cancellationToken);
            if (identityUserExists)
            {
                throw new InvalidOperationException($"Email '{request.Email}' is already registered in the system.");
            }

            // Create new Identity user first (for login capabilities)
            var identityResult = await _identityService.CreateUserAsync(request.Email, request.Password, request.Role, cancellationToken);
            if (!identityResult.Success)
            {
                throw new InvalidOperationException($"Failed to create user account: {identityResult.Error}");
            }

            // Create new user entity in custom Users table
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Role = request.Role,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add user to custom Users repository
            try
            {
                await _unitOfWork.Users.AddAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation("User created successfully in both systems with ID: {UserId}", user.Id);
            }
            catch (Exception ex)
            {
                // If custom user creation fails, clean up the Identity user
                _logger.LogError(ex, "Failed to create custom user entry, rolling back Identity user");
                await _identityService.DeleteUserByEmailAsync(request.Email, cancellationToken);
                throw;
            }

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
            _logger.LogError(ex, "Error creating user with username: {Username}", request.Username);
            throw;
        }
    }

    /// <summary>
    /// Hash password using SHA256
    /// </summary>
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
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
