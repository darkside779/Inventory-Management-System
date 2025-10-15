using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Payments.Commands.CreatePayment;

/// <summary>
/// Handler for CreatePaymentCommand
/// </summary>
public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, CreatePaymentCommandResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreatePaymentCommandHandler> _logger;

    public CreatePaymentCommandHandler(IApplicationDbContext context, ILogger<CreatePaymentCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CreatePaymentCommandResponse> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate invoice exists and can accept payments
            var invoice = await _context.CustomerInvoices
                .Include(i => i.Payments)
                .Include(i => i.Customer)
                .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);

            if (invoice == null)
            {
                return new CreatePaymentCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Invoice not found"
                };
            }

            if (invoice.Status == "Cancelled")
            {
                return new CreatePaymentCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Cannot record payment for cancelled invoice"
                };
            }

            // Calculate current paid amount and remaining balance
            var currentPaidAmount = invoice.Payments?.Sum(p => p.Amount) ?? 0;
            var remainingBalance = invoice.TotalAmount - currentPaidAmount;

            if (request.Amount > remainingBalance)
            {
                return new CreatePaymentCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"Payment amount (${request.Amount:F2}) exceeds remaining balance (${remainingBalance:F2})"
                };
            }

            // Generate payment number
            var currentYear = DateTime.Now.Year.ToString();
            var yearPrefix = $"PAY-{currentYear}";
            var lastPaymentNumber = await _context.CustomerPayments
                .Where(p => p.PaymentNumber.StartsWith(yearPrefix))
                .OrderByDescending(p => p.PaymentNumber)
                .Select(p => p.PaymentNumber)
                .FirstOrDefaultAsync(cancellationToken);

            var nextNumber = 1;
            if (!string.IsNullOrEmpty(lastPaymentNumber))
            {
                var parts = lastPaymentNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out var num))
                {
                    nextNumber = num + 1;
                }
            }

            var paymentNumber = CustomerPayment.GeneratePaymentNumber(nextNumber);

            // Auto-generate reference number based on invoice number and payment sequence
            var invoicePaymentCount = invoice.Payments?.Count() ?? 0;
            var paymentSequence = invoicePaymentCount + 1;
            var autoReferenceNumber = $"{invoice.InvoiceNumber}-PAY-{paymentSequence:D3}";
            
            // Use provided reference number or auto-generated one
            var finalReferenceNumber = !string.IsNullOrWhiteSpace(request.ReferenceNumber) 
                ? request.ReferenceNumber 
                : autoReferenceNumber;

            // Verify user exists
            var userExists = await _context.Users.AnyAsync(u => u.Id == request.RecordedByUserId, cancellationToken);
            if (!userExists)
            {
                return new CreatePaymentCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid user ID for recording payment"
                };
            }

            // Create payment
            var payment = new CustomerPayment
            {
                PaymentNumber = paymentNumber,
                CustomerId = invoice.CustomerId,
                InvoiceId = request.InvoiceId,
                Amount = request.Amount,
                PaymentDate = request.PaymentDate,
                PaymentMethod = request.PaymentMethod,
                PaymentType = "Payment",
                ReferenceNumber = finalReferenceNumber,
                Notes = request.Notes,
                RecordedByUserId = request.RecordedByUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.CustomerPayments.Add(payment);

            // Update invoice paid amount and status
            var newPaidAmount = currentPaidAmount + request.Amount;
            invoice.PaidAmount = newPaidAmount;
            invoice.UpdatedAt = DateTime.UtcNow;

            // Update invoice status based on payment
            if (newPaidAmount >= invoice.TotalAmount)
            {
                invoice.Status = "Paid";
            }
            else if (invoice.Status == "Draft")
            {
                invoice.Status = "Sent"; // Partial payment moves from draft to sent
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Payment {PaymentNumber} created for invoice {InvoiceNumber} - Amount: ${Amount}",
                paymentNumber, invoice.InvoiceNumber, request.Amount);

            return new CreatePaymentCommandResponse
            {
                IsSuccess = true,
                Payment = new PaymentDto
                {
                    Id = payment.Id,
                    PaymentNumber = payment.PaymentNumber,
                    Amount = payment.Amount,
                    PaymentDate = payment.PaymentDate,
                    PaymentMethod = payment.PaymentMethod,
                    ReferenceNumber = payment.ReferenceNumber,
                    Notes = payment.Notes
                },
                InvoiceSummary = new InvoicePaymentSummaryDto
                {
                    InvoiceId = invoice.Id,
                    InvoiceNumber = invoice.InvoiceNumber,
                    TotalAmount = invoice.TotalAmount,
                    PaidAmount = newPaidAmount,
                    RemainingBalance = invoice.TotalAmount - newPaidAmount,
                    Status = invoice.Status
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment for invoice {InvoiceId}", request.InvoiceId);
            return new CreatePaymentCommandResponse
            {
                IsSuccess = false,
                ErrorMessage = "An error occurred while recording the payment"
            };
        }
    }
}
