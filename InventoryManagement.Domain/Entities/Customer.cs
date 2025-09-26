using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Entities;

/// <summary>
/// Customer entity for managing customer information and transactions
/// </summary>
public class Customer : BaseEntity
{
    /// <summary>
    /// Customer unique identifier
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Customer code for easy reference
    /// </summary>
    [Required]
    [StringLength(20)]
    public string CustomerCode { get; set; } = string.Empty;

    /// <summary>
    /// Customer full name
    /// </summary>
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Company name (optional for business customers)
    /// </summary>
    [StringLength(100)]
    public string? CompanyName { get; set; }

    /// <summary>
    /// Email address
    /// </summary>
    [StringLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Customer address
    /// </summary>
    [StringLength(500)]
    public string? Address { get; set; }

    /// <summary>
    /// Customer type (Individual, Business, etc.)
    /// </summary>
    [Required]
    [StringLength(20)]
    public string CustomerType { get; set; } = "Individual";

    /// <summary>
    /// Current balance (positive = customer owes money, negative = credit)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; } = 0;

    /// <summary>
    /// Credit limit for the customer
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal CreditLimit { get; set; } = 0;

    /// <summary>
    /// Payment terms in days
    /// </summary>
    public int PaymentTermsDays { get; set; } = 30;

    /// <summary>
    /// Tax identification number
    /// </summary>
    [StringLength(50)]
    public string? TaxId { get; set; }

    /// <summary>
    /// Customer notes
    /// </summary>
    [StringLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Whether the customer is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Date when customer was first registered
    /// </summary>
    public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last purchase date
    /// </summary>
    public DateTime? LastPurchaseDate { get; set; }

    /// <summary>
    /// Total purchases amount
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPurchases { get; set; } = 0;

    // Navigation Properties

    /// <summary>
    /// Customer transactions (sales, returns, payments)
    /// </summary>
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    /// <summary>
    /// Customer invoices
    /// </summary>
    public virtual ICollection<CustomerInvoice> Invoices { get; set; } = new List<CustomerInvoice>();

    /// <summary>
    /// Customer payments
    /// </summary>
    public virtual ICollection<CustomerPayment> Payments { get; set; } = new List<CustomerPayment>();

    /// <summary>
    /// Validate customer data
    /// </summary>
    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(CustomerCode) || string.IsNullOrWhiteSpace(FullName))
            return false;

        if (CreditLimit < 0 || PaymentTermsDays < 0)
            return false;

        return true;
    }

    /// <summary>
    /// Check if customer has exceeded credit limit
    /// </summary>
    public bool IsOverCreditLimit => Balance > CreditLimit;

    /// <summary>
    /// Get available credit
    /// </summary>
    public decimal AvailableCredit => Math.Max(0, CreditLimit - Balance);

    /// <summary>
    /// Generate next customer code
    /// </summary>
    public static string GenerateCustomerCode(int nextNumber)
    {
        return $"CUST-{nextNumber:D6}";
    }
}
