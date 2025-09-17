using MediatR;

namespace InventoryManagement.Application.Features.Users.Commands.DeleteUser;

/// <summary>
/// Command to delete a user
/// </summary>
public class DeleteUserCommand : IRequest<bool>
{
    /// <summary>
    /// User ID to delete
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Whether to perform hard delete (true) or soft delete (false)
    /// </summary>
    public bool HardDelete { get; set; } = false;
}
