using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Application.Features.Inventory.Commands.AdjustInventory;

/// <summary>
/// Command to adjust inventory quantity
/// </summary>
public record AdjustInventoryCommand : IRequest<InventoryDto>
{
    /// <summary>
    /// Inventory adjustment data
    /// </summary>
    public AdjustInventoryDto AdjustmentDto { get; init; } = null!;

    /// <summary>
    /// User ID performing the adjustment
    /// </summary>
    public int UserId { get; init; }
}

/// <summary>
/// Handler for AdjustInventoryCommand
/// </summary>
public class AdjustInventoryCommandHandler : IRequestHandler<AdjustInventoryCommand, InventoryDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AdjustInventoryCommandHandler> _logger;

    public AdjustInventoryCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<AdjustInventoryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<InventoryDto> Handle(AdjustInventoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adjusting inventory ID: {InventoryId} by {Quantity}", 
            request.AdjustmentDto.Id, request.AdjustmentDto.AdjustmentQuantity);

        // Get existing inventory record
        var inventory = await _unitOfWork.Inventory.GetByIdAsync(request.AdjustmentDto.Id, cancellationToken);
        if (inventory == null)
        {
            throw new KeyNotFoundException($"Inventory record with ID {request.AdjustmentDto.Id} not found.");
        }

        // Validate adjustment won't result in negative inventory
        var newQuantity = inventory.Quantity + request.AdjustmentDto.AdjustmentQuantity;
        if (newQuantity < 0)
        {
            throw new InvalidOperationException($"Adjustment would result in negative inventory. Current: {inventory.Quantity}, Adjustment: {request.AdjustmentDto.AdjustmentQuantity}");
        }

        // Validate adjustment won't affect reserved quantities
        if (newQuantity < inventory.ReservedQuantity)
        {
            throw new InvalidOperationException($"Adjustment would result in quantity less than reserved amount. Reserved: {inventory.ReservedQuantity}, New Quantity: {newQuantity}");
        }

        var oldQuantity = inventory.Quantity;
        
        // Apply adjustment
        inventory.Quantity = newQuantity;
        inventory.UpdatedAt = DateTime.UtcNow;

        // Create transaction record
        var transaction = new Transaction
        {
            ProductId = inventory.ProductId,
            WarehouseId = inventory.WarehouseId,
            UserId = request.UserId,
            TransactionType = TransactionType.Adjustment,
            QuantityChanged = request.AdjustmentDto.AdjustmentQuantity,
            PreviousQuantity = oldQuantity,
            NewQuantity = newQuantity,
            Timestamp = DateTime.UtcNow,
            ReferenceNumber = request.AdjustmentDto.ReferenceNumber,
            Notes = $"Inventory adjustment: {request.AdjustmentDto.Reason}. Changed from {oldQuantity} to {newQuantity}",
            Reason = request.AdjustmentDto.Reason,
            IsActive = true
        };

        // Update inventory and create transaction
        _unitOfWork.Inventory.Update(inventory);
        await _unitOfWork.Transactions.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully adjusted inventory. Old quantity: {OldQuantity}, New quantity: {NewQuantity}", 
            oldQuantity, newQuantity);

        // Get updated inventory with navigation properties
        var updatedInventory = await _unitOfWork.Inventory.GetByIdAsync(inventory.Id, cancellationToken);
        
        return _mapper.Map<InventoryDto>(updatedInventory);
    }
}
