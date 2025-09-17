using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.Application.Features.Products.Commands.DeleteProduct;

/// <summary>
/// Command to delete a product
/// </summary>
public record DeleteProductCommand(int Id) : IRequest<bool>;

/// <summary>
/// Handler for DeleteProductCommand
/// </summary>
public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(
        IApplicationDbContext context,
        ILogger<DeleteProductCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting product with ID: {ProductId}", request.Id);

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found", request.Id);
            return false;
        }

        // Check if product has any inventory records
        var hasInventory = await _context.Inventories
            .AnyAsync(i => i.ProductId == request.Id, cancellationToken);

        if (hasInventory)
        {
            // Soft delete - set as inactive instead of hard delete
            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            _logger.LogInformation("Product has inventory records - performing soft delete");
        }
        else
        {
            // Hard delete if no inventory records
            _context.Products.Remove(product);
            _logger.LogInformation("Product has no inventory records - performing hard delete");
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully deleted product: {ProductName}", product.Name);
        return true;
    }
}
