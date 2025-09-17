using MediatR;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace InventoryManagement.Application.Features.Users.Commands.ChangePassword;

/// <summary>
/// Handler for ChangePasswordCommand
/// </summary>
public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(IUnitOfWork unitOfWork, ILogger<ChangePasswordCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Changing password for user ID: {UserId}, IsAdminReset: {IsAdminReset}", 
                request.UserId, request.IsAdminReset);

            // Get existing user
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {request.UserId} not found.");
            }

            // Verify current password if not admin reset
            if (!request.IsAdminReset && !string.IsNullOrEmpty(request.CurrentPassword))
            {
                var currentPasswordHash = HashPassword(request.CurrentPassword);
                if (user.PasswordHash != currentPasswordHash)
                {
                    throw new UnauthorizedAccessException("Current password is incorrect.");
                }
            }

            // Update password
            user.PasswordHash = HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            // Update user in repository
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Password changed successfully for user ID: {UserId}", request.UserId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user ID: {UserId}", request.UserId);
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
}
