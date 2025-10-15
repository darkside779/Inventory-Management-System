using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.ViewModels.Payment;

/// <summary>
/// ViewModel for creating a new payment
/// </summary>
public class CreatePaymentViewModel
{
    /// <summary>
    /// Invoice ID this payment is for
    /// </summary>
    [Required]
    public int InvoiceId { get; set; }

    /// <summary>
    /// Invoice number (for display)
    /// </summary>
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer name (for display)
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Invoice total amount
    /// </summary>
    public decimal InvoiceTotal { get; set; }

    /// <summary>
    /// Amount already paid
    /// </summary>
    public decimal PaidAmount { get; set; }

    /// <summary>
    /// Remaining balance
    /// </summary>
    public decimal RemainingBalance => InvoiceTotal - PaidAmount;

    /// <summary>
    /// Payment amount
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Payment amount must be greater than 0")]
    [Display(Name = "Payment Amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Payment date
    /// </summary>
    [Required]
    [Display(Name = "Payment Date")]
    public DateTime PaymentDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Payment method
    /// </summary>
    [Required]
    [Display(Name = "Payment Method")]
    public string PaymentMethod { get; set; } = "Cash";

    /// <summary>
    /// Reference number (check number, transaction ID, etc.)
    /// </summary>
    [Display(Name = "Reference Number")]
    [StringLength(100)]
    public string? ReferenceNumber { get; set; }

    /// <summary>
    /// Payment notes
    /// </summary>
    [Display(Name = "Notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Available payment methods
    /// </summary>
    public IEnumerable<SelectListItem> PaymentMethods { get; set; } = new List<SelectListItem>();

    /// <summary>
    /// Existing payments for this invoice
    /// </summary>
    public List<PaymentHistoryViewModel> PaymentHistory { get; set; } = new();
}

/// <summary>
/// ViewModel for displaying payment history
/// </summary>
public class PaymentHistoryViewModel
{
    public int Id { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public string RecordedByUser { get; set; } = string.Empty;
}
