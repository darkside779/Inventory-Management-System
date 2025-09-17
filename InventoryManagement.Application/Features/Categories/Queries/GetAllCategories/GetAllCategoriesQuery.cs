using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.Application.Features.Categories.Queries.GetAllCategories;

/// <summary>
/// Query to get all categories
/// </summary>
public record GetAllCategoriesQuery : IRequest<IEnumerable<CategoryDto>>
{
    /// <summary>
    /// Include only active categories
    /// </summary>
    public bool ActiveOnly { get; init; } = true;
}

/// <summary>
/// Handler for GetAllCategoriesQuery
/// </summary>
public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, IEnumerable<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllCategoriesQueryHandler> _logger;

    public GetAllCategoriesQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllCategoriesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving all categories (ActiveOnly: {ActiveOnly})", request.ActiveOnly);

        var categories = request.ActiveOnly
            ? await _unitOfWork.Categories.GetActiveCategoriesAsync(cancellationToken)
            : await _unitOfWork.Categories.GetAllAsync(cancellationToken);

        var result = _mapper.Map<IEnumerable<CategoryDto>>(categories);

        _logger.LogInformation("Retrieved {CategoryCount} categories", result.Count());

        return result;
    }
}
