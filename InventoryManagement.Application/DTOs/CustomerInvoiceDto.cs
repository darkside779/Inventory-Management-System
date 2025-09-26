namespace InventoryManagement.Application.DTOs;

/// <summary>
/// Data Transfer Object for Customer Invoice
/// </summary>
public class CustomerInvoiceDto
{
    /// <summary>
    /// Invoice ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Invoice number
    /// </summary>
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer ID
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Customer name
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// Invoice date
    /// </summary>
    public DateTime InvoiceDate { get; set; }

    /// <summary>
    /// Due date
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Subtotal amount
    /// </summary>
    public decimal SubTotal { get; set; }

    /// <summary>
    /// Tax amount
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Discount amount
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Total amount
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Paid amount
    /// </summary>
    public decimal PaidAmount { get; set; }

    /// <summary>
    /// Outstanding balance
    /// </summary>
    public decimal Balance => TotalAmount - PaidAmount;

    /// <summary>
    /// Invoice status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Payment terms
    /// </summary>
    public string? PaymentTerms { get; set; }

    /// <summary>
    /// Notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Created by user ID
    /// </summary>
    public int CreatedByUserId { get; set; }

    /// <summary>
    /// Created by user name
    /// </summary>
    public string? CreatedByUserName { get; set; }

    /// <summary>
    /// Is overdue
    /// </summary>
    public bool IsOverdue => DueDate < DateTime.UtcNow && Balance > 0;

    /// <summary>
    /// Is paid
    /// </summary>
    public bool IsPaid => Balance <= 0;

    /// <summary>
    /// Created date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Updated date
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
