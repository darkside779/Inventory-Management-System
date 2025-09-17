using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// Command to update an existing product
/// </summary>
public record UpdateProductCommand : IRequest<ProductDto>
{
    /// <summary>
    /// Product ID to update
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Product update data
    /// </summary>
    public UpdateProductDto ProductDto { get; init; } = null!;
}

/// <summary>
/// Handler for UpdateProductCommand
/// </summary>
public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating product with ID: {ProductId}", request.Id);

        // Find the existing product
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {request.Id} not found.");
        }

        // Validate category exists
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.ProductDto.CategoryId, cancellationToken);
        if (category == null)
        {
            throw new KeyNotFoundException($"Category with ID {request.ProductDto.CategoryId} not found.");
        }

        // Validate supplier exists if provided
        if (request.ProductDto.SupplierId.HasValue)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.Id == request.ProductDto.SupplierId.Value, cancellationToken);
            if (supplier == null)
            {
                throw new KeyNotFoundException($"Supplier with ID {request.ProductDto.SupplierId.Value} not found.");
            }
        }

        // Check if SKU already exists for another product
        var existingSkuProduct = await _context.Products
            .FirstOrDefaultAsync(p => p.SKU == request.ProductDto.SKU && p.Id != request.Id, cancellationToken);
        if (existingSkuProduct != null)
        {
            throw new InvalidOperationException($"Product with SKU '{request.ProductDto.SKU}' already exists.");
        }

        // Check if barcode already exists for another product (if provided)
        if (!string.IsNullOrEmpty(request.ProductDto.Barcode))
        {
            var existingBarcodeProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Barcode == request.ProductDto.Barcode && p.Id != request.Id, cancellationToken);
            if (existingBarcodeProduct != null)
            {
                throw new InvalidOperationException($"Product with barcode '{request.ProductDto.Barcode}' already exists.");
            }
        }

        // Update product properties
        product.Name = request.ProductDto.Name;
        product.SKU = request.ProductDto.SKU;
        product.Description = request.ProductDto.Description;
        product.Price = request.ProductDto.Price;
        product.Cost = request.ProductDto.Cost;
        product.CategoryId = request.ProductDto.CategoryId;
        product.SupplierId = request.ProductDto.SupplierId;
        product.LowStockThreshold = request.ProductDto.LowStockThreshold;
        product.Unit = request.ProductDto.Unit;
        product.Barcode = request.ProductDto.Barcode;
        product.Weight = request.ProductDto.Weight;
        product.Dimensions = request.ProductDto.Dimensions;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully updated product: {ProductName}", product.Name);

        // Return updated product as DTO
        return _mapper.Map<ProductDto>(product);
    }
}
