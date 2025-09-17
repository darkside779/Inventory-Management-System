using MediatR;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.Application.Features.Categories.Commands.DeleteCategory;

/// <summary>
/// Command to delete a category
/// </summary>
public record DeleteCategoryCommand : IRequest<bool>
{
    /// <summary>
    /// Category ID to delete
    /// </summary>
    public int Id { get; init; }
}

/// <summary>
/// Handler for DeleteCategoryCommand
/// </summary>
public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCategoryCommandHandler> _logger;

    public DeleteCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteCategoryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting category with ID: {CategoryId}", request.Id);

        // Get existing category
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found for deletion", request.Id);
            return false;
        }

        // Check if category has products
        var productsInCategory = await _unitOfWork.Products.GetAsync(
            filter: p => p.CategoryId == request.Id,
            cancellationToken: cancellationToken);
        var productCount = productsInCategory.Count();
        
        if (productCount > 0)
        {
            throw new InvalidOperationException($"Cannot delete category '{category.Name}' because it has {productCount} associated products.");
        }

        // Soft delete the category
        var result = await _unitOfWork.Categories.SoftDeleteAsync(request.Id, cancellationToken);
        if (result)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully deleted category with ID: {CategoryId}", request.Id);
        }

        return result;
    }
}
