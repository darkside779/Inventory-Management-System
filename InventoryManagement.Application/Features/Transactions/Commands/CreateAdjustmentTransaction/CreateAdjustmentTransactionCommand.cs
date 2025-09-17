using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Transactions.Commands.CreateAdjustmentTransaction;

/// <summary>
/// Command to create an inventory adjustment transaction
/// </summary>
public class CreateAdjustmentTransactionCommand : IRequest<CreateAdjustmentTransactionCommandResponse>
{
    /// <summary>
    /// Product ID
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Warehouse ID
    /// </summary>
    public int WarehouseId { get; set; }

    /// <summary>
    /// User ID performing the adjustment
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Quantity adjustment (positive for increase, negative for decrease)
    /// </summary>
    public int QuantityAdjustment { get; set; }

    /// <summary>
    /// Reason for the adjustment
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Reference number for the adjustment
    /// </summary>
    public string? ReferenceNumber { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Response for CreateAdjustmentTransactionCommand
/// </summary>
public class CreateAdjustmentTransactionCommandResponse
{
    /// <summary>
    /// Created transaction
    /// </summary>
    public TransactionDto? Transaction { get; set; }

    /// <summary>
    /// Updated inventory information
    /// </summary>
    public InventoryDto? UpdatedInventory { get; set; }

    /// <summary>
    /// Indicates if the command was successful
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Error message if command failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Handler for CreateAdjustmentTransactionCommand
/// </summary>
public class CreateAdjustmentTransactionCommandHandler : IRequestHandler<CreateAdjustmentTransactionCommand, CreateAdjustmentTransactionCommandResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateAdjustmentTransactionCommandHandler> _logger;

    public CreateAdjustmentTransactionCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateAdjustmentTransactionCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateAdjustmentTransactionCommandResponse> Handle(CreateAdjustmentTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating adjustment transaction for Product: {ProductId}, Warehouse: {WarehouseId}, Adjustment: {QuantityAdjustment}", 
                request.ProductId, request.WarehouseId, request.QuantityAdjustment);

            // Validate inputs
            if (request.QuantityAdjustment == 0)
            {
                return new CreateAdjustmentTransactionCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Quantity adjustment cannot be zero."
                };
            }

            // Verify product exists
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken: cancellationToken);
            if (product == null)
            {
                return new CreateAdjustmentTransactionCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Product not found."
                };
            }

            // Verify warehouse exists
            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(request.WarehouseId, cancellationToken: cancellationToken);
            if (warehouse == null)
            {
                return new CreateAdjustmentTransactionCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Warehouse not found."
                };
            }

            // Verify user exists
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken: cancellationToken);
            if (user == null)
            {
                return new CreateAdjustmentTransactionCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "User not found."
                };
            }

            // Get current inventory
            var inventories = await _unitOfWork.Inventory.GetAsync(
                filter: i => i.ProductId == request.ProductId && i.WarehouseId == request.WarehouseId,
                cancellationToken: cancellationToken);
            var inventory = inventories.FirstOrDefault();

            var previousQuantity = inventory?.Quantity ?? 0;
            var newQuantity = previousQuantity + request.QuantityAdjustment;

            // Check if adjustment would result in negative stock
            if (newQuantity < 0)
            {
                return new CreateAdjustmentTransactionCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"Adjustment would result in negative stock. Current: {previousQuantity}, Adjustment: {request.QuantityAdjustment}"
                };
            }

            // Create transaction
            var transaction = Transaction.CreateAdjustment(
                request.ProductId,
                request.WarehouseId,
                request.UserId,
                request.QuantityAdjustment,
                previousQuantity,
                request.Reason,
                request.ReferenceNumber);

            transaction.Notes = request.Notes;

            // Validate transaction
            if (!transaction.IsValid())
            {
                return new CreateAdjustmentTransactionCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Transaction validation failed."
                };
            }

            // Add transaction
            await _unitOfWork.Transactions.AddAsync(transaction, cancellationToken);

            // Update or create inventory
            if (inventory == null)
            {
                // Create new inventory record if it doesn't exist
                inventory = new Domain.Entities.Inventory
                {
                    ProductId = request.ProductId,
                    WarehouseId = request.WarehouseId,
                    Quantity = Math.Max(0, request.QuantityAdjustment),
                    ReservedQuantity = 0,
                    IsActive = true
                };
                await _unitOfWork.Inventory.AddAsync(inventory, cancellationToken);
            }
            else
            {
                // Use the domain method to adjust quantity
                if (!inventory.AdjustQuantity(request.QuantityAdjustment))
                {
                    return new CreateAdjustmentTransactionCommandResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "Failed to adjust inventory quantity."
                    };
                }
            }

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Load transaction with navigation properties
            var transactionWithIncludes = await _unitOfWork.Transactions.GetAsync(
                filter: t => t.Id == transaction.Id,
                includeProperties: "Product,Warehouse,User",
                cancellationToken: cancellationToken);
            transaction = transactionWithIncludes.FirstOrDefault() ?? transaction;

            // Load inventory with navigation properties
            var inventoryWithIncludes = await _unitOfWork.Inventory.GetAsync(
                filter: i => i.Id == inventory.Id,
                includeProperties: "Product,Warehouse",
                cancellationToken: cancellationToken);
            inventory = inventoryWithIncludes.FirstOrDefault() ?? inventory;

            // Map to DTOs
            var transactionDto = _mapper.Map<TransactionDto>(transaction);
            var inventoryDto = _mapper.Map<InventoryDto>(inventory);

            _logger.LogInformation("Successfully created adjustment transaction with ID: {TransactionId}", transaction.Id);

            return new CreateAdjustmentTransactionCommandResponse
            {
                Transaction = transactionDto,
                UpdatedInventory = inventoryDto,
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating adjustment transaction");

            return new CreateAdjustmentTransactionCommandResponse
            {
                IsSuccess = false,
                ErrorMessage = "An error occurred while creating the adjustment transaction."
            };
        }
    }
}
