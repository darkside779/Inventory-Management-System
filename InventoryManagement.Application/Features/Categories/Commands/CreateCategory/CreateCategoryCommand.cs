using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.Categories.Commands.CreateCategory;

/// <summary>
/// Command to create a new category
/// </summary>
public record CreateCategoryCommand : IRequest<CategoryDto>
{
    /// <summary>
    /// Category creation data
    /// </summary>
    public CreateCategoryDto CategoryDto { get; init; } = null!;
}

/// <summary>
/// Handler for CreateCategoryCommand
/// </summary>
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;

    public CreateCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateCategoryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new category with name: {CategoryName}", request.CategoryDto.Name);

        // Check if category name already exists
        var existingCategory = await _unitOfWork.Categories.GetByNameAsync(request.CategoryDto.Name, cancellationToken);
        if (existingCategory != null)
        {
            throw new InvalidOperationException($"Category with name '{request.CategoryDto.Name}' already exists.");
        }

        // Map DTO to entity
        var category = _mapper.Map<Category>(request.CategoryDto);

        // Add to repository
        await _unitOfWork.Categories.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created category with ID: {CategoryId}", category.Id);

        // Return mapped DTO
        return _mapper.Map<CategoryDto>(category);
    }
}
