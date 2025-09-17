using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.Application.Features.Categories.Queries.GetCategoryById;

/// <summary>
/// Query to get a category by ID
/// </summary>
public record GetCategoryByIdQuery : IRequest<CategoryDto?>
{
    /// <summary>
    /// Category ID
    /// </summary>
    public int Id { get; init; }
}

/// <summary>
/// Handler for GetCategoryByIdQuery
/// </summary>
public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCategoryByIdQueryHandler> _logger;

    public GetCategoryByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetCategoryByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving category with ID: {CategoryId}", request.Id);

        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, cancellationToken);
        
        if (category == null)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found", request.Id);
            return null;
        }

        var result = _mapper.Map<CategoryDto>(category);

        _logger.LogInformation("Successfully retrieved category: {CategoryName}", result.Name);

        return result;
    }
}
