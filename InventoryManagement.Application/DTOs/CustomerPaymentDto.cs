namespace InventoryManagement.Application.DTOs;

/// <summary>
/// Data Transfer Object for Customer Payment
/// </summary>
public class CustomerPaymentDto
{
    /// <summary>
    /// Payment ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Payment number
    /// </summary>
    public string PaymentNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer ID
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Customer name
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// Invoice ID (if payment is for specific invoice)
    /// </summary>
    public int? InvoiceId { get; set; }

    /// <summary>
    /// Invoice number
    /// </summary>
    public string? InvoiceNumber { get; set; }

    /// <summary>
    /// Payment date
    /// </summary>
    public DateTime PaymentDate { get; set; }

    /// <summary>
    /// Payment amount
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Payment method
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// Payment type
    /// </summary>
    public string PaymentType { get; set; } = string.Empty;

    /// <summary>
    /// Reference number
    /// </summary>
    public string? ReferenceNumber { get; set; }

    /// <summary>
    /// Notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Recorded by user ID
    /// </summary>
    public int RecordedByUserId { get; set; }

    /// <summary>
    /// Recorded by user name
    /// </summary>
    public string? RecordedByUserName { get; set; }

    /// <summary>
    /// Created date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Updated date
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
