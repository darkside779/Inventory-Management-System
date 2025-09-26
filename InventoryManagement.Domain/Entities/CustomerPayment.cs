using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Entities;

/// <summary>
/// Customer payment entity for tracking payments and credits
/// </summary>
public class CustomerPayment : BaseEntity
{
    /// <summary>
    /// Payment unique identifier
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Payment number/reference
    /// </summary>
    [Required]
    [StringLength(50)]
    public string PaymentNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer ID
    /// </summary>
    [ForeignKey(nameof(Customer))]
    public int CustomerId { get; set; }

    /// <summary>
    /// Invoice ID (if payment is for specific invoice)
    /// </summary>
    [ForeignKey(nameof(Invoice))]
    public int? InvoiceId { get; set; }

    /// <summary>
    /// Payment date
    /// </summary>
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Payment amount
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Payment method (Cash, Card, Bank Transfer, etc.)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string PaymentMethod { get; set; } = "Cash";

    /// <summary>
    /// Payment type (Payment, Refund, Credit)
    /// </summary>
    [Required]
    [StringLength(20)]
    public string PaymentType { get; set; } = "Payment";

    /// <summary>
    /// Reference number (check number, transaction ID, etc.)
    /// </summary>
    [StringLength(100)]
    public string? ReferenceNumber { get; set; }

    /// <summary>
    /// Payment notes
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// User who recorded the payment
    /// </summary>
    [ForeignKey(nameof(RecordedByUser))]
    public int RecordedByUserId { get; set; }

    // Navigation Properties

    /// <summary>
    /// Customer
    /// </summary>
    public virtual Customer Customer { get; set; } = null!;

    /// <summary>
    /// Invoice (if applicable)
    /// </summary>
    public virtual CustomerInvoice? Invoice { get; set; }

    /// <summary>
    /// User who recorded the payment
    /// </summary>
    public virtual User RecordedByUser { get; set; } = null!;

    /// <summary>
    /// Generate next payment number
    /// </summary>
    public static string GeneratePaymentNumber(int nextNumber)
    {
        return $"PAY-{DateTime.Now:yyyy}-{nextNumber:D6}";
    }
}
