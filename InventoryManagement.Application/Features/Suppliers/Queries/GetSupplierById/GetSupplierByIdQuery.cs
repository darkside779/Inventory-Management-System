using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.Application.Features.Suppliers.Queries.GetSupplierById;

/// <summary>
/// Query to retrieve a specific supplier by ID
/// </summary>
public class GetSupplierByIdQuery : IRequest<GetSupplierByIdQueryResponse>
{
    /// <summary>
    /// Supplier ID to retrieve
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Include products in the response
    /// </summary>
    public bool IncludeProducts { get; set; } = false;

    public GetSupplierByIdQuery(int id, bool includeProducts = false)
    {
        Id = id;
        IncludeProducts = includeProducts;
    }
}

/// <summary>
/// Response for GetSupplierByIdQuery
/// </summary>
public class GetSupplierByIdQueryResponse
{
    /// <summary>
    /// Supplier data
    /// </summary>
    public SupplierDto? Supplier { get; set; }

    /// <summary>
    /// Success flag
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if any
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Handler for GetSupplierByIdQuery
/// </summary>
public class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, GetSupplierByIdQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSupplierByIdQueryHandler> _logger;

    public GetSupplierByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSupplierByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetSupplierByIdQueryResponse> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving supplier with ID: {SupplierId}", request.Id);

            var includeProperties = request.IncludeProducts ? "Products,Products.Category" : string.Empty;
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(request.Id, cancellationToken);

            if (supplier == null)
            {
                _logger.LogWarning("Supplier with ID {SupplierId} not found", request.Id);
                return new GetSupplierByIdQueryResponse
                {
                    Success = false,
                    ErrorMessage = $"Supplier with ID {request.Id} not found"
                };
            }

            var supplierDto = _mapper.Map<SupplierDto>(supplier);

            _logger.LogInformation("Successfully retrieved supplier: {SupplierName}", supplier.Name);

            return new GetSupplierByIdQueryResponse
            {
                Supplier = supplierDto,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving supplier with ID: {SupplierId}", request.Id);
            return new GetSupplierByIdQueryResponse
            {
                Success = false,
                ErrorMessage = $"Error retrieving supplier: {ex.Message}"
            };
        }
    }
}
