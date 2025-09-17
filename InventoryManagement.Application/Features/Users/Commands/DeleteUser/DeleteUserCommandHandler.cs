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

    public DeleteUserCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
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
                // Hard delete - remove from database
                _unitOfWork.Users.Delete(user);
                _logger.LogInformation("User with ID {UserId} marked for hard deletion", request.Id);
            }
            else
            {
                // Soft delete - mark as inactive
                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Users.Update(user);
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
