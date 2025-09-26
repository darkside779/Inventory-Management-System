using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Entities;

/// <summary>
/// Customer invoice entity for sales transactions
/// </summary>
public class CustomerInvoice : BaseEntity
{
    /// <summary>
    /// Invoice unique identifier
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Invoice number
    /// </summary>
    [Required]
    [StringLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer ID
    /// </summary>
    [ForeignKey(nameof(Customer))]
    public int CustomerId { get; set; }

    /// <summary>
    /// Invoice date
    /// </summary>
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Due date for payment
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Subtotal amount (before tax)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    /// <summary>
    /// Tax amount
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Discount amount
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Total amount (subtotal + tax - discount)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Amount paid
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal PaidAmount { get; set; }

    /// <summary>
    /// Outstanding balance
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance => TotalAmount - PaidAmount;

    /// <summary>
    /// Invoice status
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Paid, Overdue, Cancelled

    /// <summary>
    /// Payment terms
    /// </summary>
    [StringLength(100)]
    public string? PaymentTerms { get; set; }

    /// <summary>
    /// Invoice notes
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// User who created the invoice
    /// </summary>
    [ForeignKey(nameof(CreatedByUser))]
    public int CreatedByUserId { get; set; }

    // Navigation Properties

    /// <summary>
    /// Customer
    /// </summary>
    public virtual Customer Customer { get; set; } = null!;

    /// <summary>
    /// User who created the invoice
    /// </summary>
    public virtual User CreatedByUser { get; set; } = null!;

    /// <summary>
    /// Invoice line items
    /// </summary>
    public virtual ICollection<CustomerInvoiceItem> Items { get; set; } = new List<CustomerInvoiceItem>();

    /// <summary>
    /// Related payments
    /// </summary>
    public virtual ICollection<CustomerPayment> Payments { get; set; } = new List<CustomerPayment>();

    /// <summary>
    /// Check if invoice is overdue
    /// </summary>
    public bool IsOverdue => DueDate < DateTime.UtcNow && Balance > 0;

    /// <summary>
    /// Check if invoice is fully paid
    /// </summary>
    public bool IsPaid => Balance <= 0;

    /// <summary>
    /// Generate next invoice number
    /// </summary>
    public static string GenerateInvoiceNumber(int nextNumber)
    {
        return $"INV-{DateTime.Now:yyyy}-{nextNumber:D6}";
    }
}
