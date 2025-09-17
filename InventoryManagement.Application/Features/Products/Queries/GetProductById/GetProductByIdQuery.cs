using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.Application.Features.Products.Queries.GetProductById;

/// <summary>
/// Query to get a product by ID
/// </summary>
public record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;

/// <summary>
/// Handler for GetProductByIdQuery
/// </summary>
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    public GetProductByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ILogger<GetProductByIdQueryHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving product with ID: {ProductId}", request.Id);

        var product = await _context.Products
            .Where(p => p.Id == request.Id)
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(cancellationToken);

        if (product == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found", request.Id);
            return null;
        }

        var result = _mapper.Map<ProductDto>(product);
        
        _logger.LogInformation("Successfully retrieved product: {ProductName}", product.Name);
        
        return result;
    }
}
