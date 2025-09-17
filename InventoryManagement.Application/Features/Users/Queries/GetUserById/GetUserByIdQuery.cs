using MediatR;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Features.Users.Queries.GetUserById;

/// <summary>
/// Query to get user by ID
/// </summary>
public class GetUserByIdQuery : IRequest<UserDto?>
{
    /// <summary>
    /// User ID
    /// </summary>
    public int Id { get; set; }

    public GetUserByIdQuery(int id)
    {
        Id = id;
    }
}
