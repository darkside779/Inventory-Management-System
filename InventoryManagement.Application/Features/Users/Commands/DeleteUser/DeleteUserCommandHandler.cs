using MediatR;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.Application.Features.Users.Commands.DeleteUser;

/// <summary>
/// Handler for DeleteUserCommand
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteUserCommandHandler> _logger;
    private readonly IIdentityService _identityService;

    public DeleteUserCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteUserCommandHandler> logger, IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _identityService = identityService;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting user with ID: {UserId}, Hard Delete: {HardDelete}", request.Id, request.HardDelete);

            // Get existing user
            var user = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for deletion", request.Id);
                return false;
            }

            // Check if user has related transactions
            var hasTransactions = user.Transactions?.Any() == true;
            if (hasTransactions && request.HardDelete)
            {
                throw new InvalidOperationException("Cannot hard delete user with existing transactions. Use soft delete instead.");
            }

            if (request.HardDelete)
            {
                // Hard delete - remove from both systems
                _unitOfWork.Users.Delete(user);
                
                // Also delete from Identity system
                var identityDeleteResult = await _identityService.DeleteUserByEmailAsync(user.Email, cancellationToken);
                if (!identityDeleteResult)
                {
                    _logger.LogWarning("Failed to delete Identity user for email: {Email}", user.Email);
                }
                
                _logger.LogInformation("User with ID {UserId} marked for hard deletion from both systems", request.Id);
            }
            else
            {
                // Soft delete - mark as inactive in custom table and disable in Identity
                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Users.Update(user);
                
                // Note: For soft delete, we keep the Identity user but we could disable it
                // For now, we'll just mark the custom user as inactive
                _logger.LogInformation("User with ID {UserId} deactivated (soft delete)", request.Id);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User deletion operation completed successfully for ID: {UserId}", request.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID: {UserId}", request.Id);
            throw;
        }
    }
}
