using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.CustomerInvoices.Commands;

/// <summary>
/// Command to create a new customer invoice
/// </summary>
public class CreateInvoiceCommand : IRequest<CreateInvoiceCommandResponse>
{
    /// <summary>
    /// Customer ID
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Invoice date
    /// </summary>
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Due date
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Payment terms
    /// </summary>
    public string? PaymentTerms { get; set; }

    /// <summary>
    /// Notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Invoice items
    /// </summary>
    public List<CreateInvoiceItemDto> Items { get; set; } = new();

    /// <summary>
    /// Created by user ID
    /// </summary>
    public int CreatedByUserId { get; set; }
}

/// <summary>
/// DTO for creating invoice items
/// </summary>
public class CreateInvoiceItemDto
{
    /// <summary>
    /// Product ID
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Quantity
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Unit price
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Discount percentage
    /// </summary>
    public decimal DiscountPercentage { get; set; } = 0;

    /// <summary>
    /// Tax percentage
    /// </summary>
    public decimal TaxPercentage { get; set; } = 0;

    /// <summary>
    /// Description
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// Response for CreateInvoiceCommand
/// </summary>
public class CreateInvoiceCommandResponse
{
    /// <summary>
    /// Created invoice
    /// </summary>
    public CustomerInvoiceDto? Invoice { get; set; }

    /// <summary>
    /// Success flag
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Handler for CreateInvoiceCommand
/// </summary>
public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, CreateInvoiceCommandResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateInvoiceCommandHandler> _logger;

    public CreateInvoiceCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateInvoiceCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateInvoiceCommandResponse> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating invoice for customer: {CustomerId}", request.CustomerId);

            // Validate customer exists
            var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId, cancellationToken: cancellationToken);
            if (customer == null)
            {
                return new CreateInvoiceCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Customer not found"
                };
            }

            // Validate invoice items
            if (!request.Items.Any())
            {
                return new CreateInvoiceCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Invoice must have at least one item"
                };
            }

            // Generate invoice number
            var invoiceCount = await _unitOfWork.CustomerInvoices.CountAsync(cancellationToken: cancellationToken);
            var invoiceNumber = CustomerInvoice.GenerateInvoiceNumber(invoiceCount + 1);

            // Calculate totals
            decimal subTotal = 0;
            decimal totalTax = 0;
            decimal totalDiscount = 0;

            var invoiceItems = new List<CustomerInvoiceItem>();

            foreach (var itemDto in request.Items)
            {
                // Validate product exists
                var product = await _unitOfWork.Products.GetByIdAsync(itemDto.ProductId, cancellationToken: cancellationToken);
                if (product == null)
                {
                    return new CreateInvoiceCommandResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Product with ID {itemDto.ProductId} not found"
                    };
                }

                var lineTotal = itemDto.Quantity * itemDto.UnitPrice;
                var discountAmount = lineTotal * (itemDto.DiscountPercentage / 100);
                var taxableAmount = lineTotal - discountAmount;
                var taxAmount = taxableAmount * (itemDto.TaxPercentage / 100);

                subTotal += lineTotal;
                totalDiscount += discountAmount;
                totalTax += taxAmount;

                invoiceItems.Add(new CustomerInvoiceItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    DiscountPercentage = itemDto.DiscountPercentage,
                    TaxPercentage = itemDto.TaxPercentage,
                    Description = itemDto.Description ?? product.Name,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }

            var totalAmount = subTotal - totalDiscount + totalTax;

            // Create invoice
            var invoice = new CustomerInvoice
            {
                InvoiceNumber = invoiceNumber,
                CustomerId = request.CustomerId,
                InvoiceDate = request.InvoiceDate,
                DueDate = request.DueDate != default ? request.DueDate : request.InvoiceDate.AddDays(customer.PaymentTermsDays),
                SubTotal = subTotal,
                TaxAmount = totalTax,
                DiscountAmount = totalDiscount,
                TotalAmount = totalAmount,
                PaidAmount = 0,
                Status = "Draft",
                PaymentTerms = request.PaymentTerms ?? $"Net {customer.PaymentTermsDays} days",
                Notes = request.Notes,
                CreatedByUserId = request.CreatedByUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true,
                Items = invoiceItems
            };

            // Save invoice
            await _unitOfWork.CustomerInvoices.AddAsync(invoice, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Update customer balance
            customer.Balance += totalAmount;
            customer.TotalPurchases += totalAmount;
            customer.LastPurchaseDate = DateTime.UtcNow;
            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Invoice created successfully: {InvoiceNumber}", invoice.InvoiceNumber);

            return new CreateInvoiceCommandResponse
            {
                IsSuccess = true,
                Invoice = _mapper.Map<CustomerInvoiceDto>(invoice)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invoice for customer: {CustomerId}", request.CustomerId);
            return new CreateInvoiceCommandResponse
            {
                IsSuccess = false,
                ErrorMessage = "An error occurred while creating the invoice"
            };
        }
    }
}
