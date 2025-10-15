using MediatR;

namespace InventoryManagement.Application.Features.Payments.Commands.CreatePayment;

/// <summary>
/// Command to create a new payment
/// </summary>
public class CreatePaymentCommand : IRequest<CreatePaymentCommandResponse>
{
    /// <summary>
    /// Invoice ID this payment is for
    /// </summary>
    public int InvoiceId { get; set; }

    /// <summary>
    /// Payment amount
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Payment date
    /// </summary>
    public DateTime PaymentDate { get; set; }

    /// <summary>
    /// Payment method
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// Reference number
    /// </summary>
    public string? ReferenceNumber { get; set; }

    /// <summary>
    /// Payment notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// User ID who is recording the payment
    /// </summary>
    public int RecordedByUserId { get; set; }
}

/// <summary>
/// Response for CreatePaymentCommand
/// </summary>
public class CreatePaymentCommandResponse
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Created payment information
    /// </summary>
    public PaymentDto? Payment { get; set; }

    /// <summary>
    /// Updated invoice information
    /// </summary>
    public InvoicePaymentSummaryDto? InvoiceSummary { get; set; }
}

/// <summary>
/// Payment DTO
/// </summary>
public class PaymentDto
{
    public int Id { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Invoice payment summary DTO
/// </summary>
public class InvoicePaymentSummaryDto
{
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingBalance { get; set; }
    public string Status { get; set; } = string.Empty;
}
