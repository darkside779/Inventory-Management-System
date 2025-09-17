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

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating new user with username: {Username}", request.Username);

            // Check if username already exists
            var existingUserByUsername = await _unitOfWork.Users.GetByUsernameAsync(request.Username, cancellationToken);
            if (existingUserByUsername != null)
            {
                throw new InvalidOperationException($"Username '{request.Username}' is already taken.");
            }

            // Check if email already exists
            var existingUserByEmail = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUserByEmail != null)
            {
                throw new InvalidOperationException($"Email '{request.Email}' is already registered.");
            }

            // Create new user entity
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

            // Add user to repository
            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User created successfully with ID: {UserId}", user.Id);

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
