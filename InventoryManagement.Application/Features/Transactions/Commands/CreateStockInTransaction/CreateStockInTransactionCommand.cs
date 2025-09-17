using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Transactions.Commands.CreateStockInTransaction;

/// <summary>
/// Command to create a stock in transaction
/// </summary>
public class CreateStockInTransactionCommand : IRequest<CreateStockInTransactionCommandResponse>
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
    /// User ID performing the transaction
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Quantity to add to stock
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Unit cost of the items
    /// </summary>
    public decimal? UnitCost { get; set; }

    /// <summary>
    /// Reason for the stock in
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Reference number (PO, invoice, etc.)
    /// </summary>
    public string? ReferenceNumber { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Response for CreateStockInTransactionCommand
/// </summary>
public class CreateStockInTransactionCommandResponse
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
/// Handler for CreateStockInTransactionCommand
/// </summary>
public class CreateStockInTransactionCommandHandler : IRequestHandler<CreateStockInTransactionCommand, CreateStockInTransactionCommandResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateStockInTransactionCommandHandler> _logger;

    public CreateStockInTransactionCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateStockInTransactionCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateStockInTransactionCommandResponse> Handle(CreateStockInTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating stock in transaction for Product: {ProductId}, Warehouse: {WarehouseId}, Quantity: {Quantity}", 
                request.ProductId, request.WarehouseId, request.Quantity);

            // Validate inputs
            if (request.Quantity <= 0)
            {
                return new CreateStockInTransactionCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Quantity must be greater than zero."
                };
            }

            // Verify product exists
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken: cancellationToken);
            if (product == null)
            {
                return new CreateStockInTransactionCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Product not found."
                };
            }

            // Verify warehouse exists
            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(request.WarehouseId, cancellationToken: cancellationToken);
            if (warehouse == null)
            {
                return new CreateStockInTransactionCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Warehouse not found."
                };
            }

            // Verify user exists
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken: cancellationToken);
            if (user == null)
            {
                return new CreateStockInTransactionCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "User not found."
                };
            }

            // Get current inventory or create new one
            var inventories = await _unitOfWork.Inventory.GetAsync(
                filter: i => i.ProductId == request.ProductId && i.WarehouseId == request.WarehouseId,
                cancellationToken: cancellationToken);
            var inventory = inventories.FirstOrDefault();

            var previousQuantity = inventory?.Quantity ?? 0;
            var newQuantity = previousQuantity + request.Quantity;

            // Create transaction
            var transaction = Transaction.CreateStockIn(
                request.ProductId,
                request.WarehouseId,
                request.UserId,
                request.Quantity,
                previousQuantity,
                request.UnitCost,
                request.Reason,
                request.ReferenceNumber);

            transaction.Notes = request.Notes;

            // Validate transaction
            if (!transaction.IsValid())
            {
                return new CreateStockInTransactionCommandResponse
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
                inventory = new Domain.Entities.Inventory
                {
                    ProductId = request.ProductId,
                    WarehouseId = request.WarehouseId,
                    Quantity = request.Quantity,
                    ReservedQuantity = 0,
                    IsActive = true
                };
                await _unitOfWork.Inventory.AddAsync(inventory, cancellationToken);
            }
            else
            {
                // Use the domain method to adjust quantity
                if (!inventory.AdjustQuantity(request.Quantity))
                {
                    return new CreateStockInTransactionCommandResponse
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

            _logger.LogInformation("Successfully created stock in transaction with ID: {TransactionId}", transaction.Id);

            return new CreateStockInTransactionCommandResponse
            {
                Transaction = transactionDto,
                UpdatedInventory = inventoryDto,
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating stock in transaction");

            return new CreateStockInTransactionCommandResponse
            {
                IsSuccess = false,
                ErrorMessage = "An error occurred while creating the stock in transaction."
            };
        }
    }
}
