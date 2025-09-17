using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.Categories.Commands.UpdateCategory;

/// <summary>
/// Command to update an existing category
/// </summary>
public record UpdateCategoryCommand : IRequest<CategoryDto>
{
    /// <summary>
    /// Category update data
    /// </summary>
    public UpdateCategoryDto CategoryDto { get; init; } = null!;
}

/// <summary>
/// Handler for UpdateCategoryCommand
/// </summary>
public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateCategoryCommandHandler> _logger;

    public UpdateCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateCategoryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating category with ID: {CategoryId}", request.CategoryDto.Id);

        // Get existing category
        var existingCategory = await _unitOfWork.Categories.GetByIdAsync(request.CategoryDto.Id, cancellationToken);
        if (existingCategory == null)
        {
            throw new KeyNotFoundException($"Category with ID {request.CategoryDto.Id} not found.");
        }

        // Check if new name conflicts with existing category
        if (existingCategory.Name != request.CategoryDto.Name)
        {
            var nameExists = await _unitOfWork.Categories.IsNameUniqueAsync(request.CategoryDto.Name, request.CategoryDto.Id, cancellationToken);
            if (!nameExists)
            {
                throw new InvalidOperationException($"Category with name '{request.CategoryDto.Name}' already exists.");
            }
        }

        // Map updates to existing entity
        _mapper.Map(request.CategoryDto, existingCategory);

        // Update in repository
        _unitOfWork.Categories.Update(existingCategory);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully updated category with ID: {CategoryId}", existingCategory.Id);

        // Return mapped DTO
        return _mapper.Map<CategoryDto>(existingCategory);
    }
}
