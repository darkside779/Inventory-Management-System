using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// Command to create a new product
/// </summary>
public record CreateProductCommand : IRequest<ProductDto>
{
    /// <summary>
    /// Product creation data
    /// </summary>
    public CreateProductDto ProductDto { get; init; } = null!;
}

/// <summary>
/// Handler for CreateProductCommand
/// </summary>
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new product with SKU: {ProductSKU}", request.ProductDto.SKU);

        // Validate category exists
        var category = await _unitOfWork.Categories.GetByIdAsync(request.ProductDto.CategoryId, cancellationToken);
        if (category == null)
        {
            throw new KeyNotFoundException($"Category with ID {request.ProductDto.CategoryId} not found.");
        }

        // Validate supplier exists if provided
        if (request.ProductDto.SupplierId.HasValue)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(request.ProductDto.SupplierId.Value, cancellationToken);
            if (supplier == null)
            {
                throw new KeyNotFoundException($"Supplier with ID {request.ProductDto.SupplierId.Value} not found.");
            }
        }

        // Check if SKU already exists
        var existingProduct = await _unitOfWork.Products.GetBySkuAsync(request.ProductDto.SKU, cancellationToken);
        if (existingProduct != null)
        {
            throw new InvalidOperationException($"Product with SKU '{request.ProductDto.SKU}' already exists.");
        }

        // Check if barcode already exists (if provided)
        if (!string.IsNullOrEmpty(request.ProductDto.Barcode))
        {
            var existingBarcode = await _unitOfWork.Products.GetByBarcodeAsync(request.ProductDto.Barcode, cancellationToken);
            if (existingBarcode != null)
            {
                throw new InvalidOperationException($"Product with barcode '{request.ProductDto.Barcode}' already exists.");
            }
        }

        // Map DTO to entity
        var product = _mapper.Map<Product>(request.ProductDto);

        // Add to repository
        await _unitOfWork.Products.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created product with ID: {ProductId}", product.Id);

        // Get the created product with navigation properties
        var createdProduct = await _unitOfWork.Products.GetByIdAsync(product.Id, cancellationToken);
        
        // Return mapped DTO
        return _mapper.Map<ProductDto>(createdProduct);
    }
}
