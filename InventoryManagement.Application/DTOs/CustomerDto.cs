namespace InventoryManagement.Application.DTOs;

/// <summary>
/// Data Transfer Object for Customer
/// </summary>
public class CustomerDto
{
    /// <summary>
    /// Customer ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Customer code
    /// </summary>
    public string CustomerCode { get; set; } = string.Empty;

    /// <summary>
    /// Full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Company name
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// Email address
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Customer type
    /// </summary>
    public string CustomerType { get; set; } = string.Empty;

    /// <summary>
    /// Current balance
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Credit limit
    /// </summary>
    public decimal CreditLimit { get; set; }

    /// <summary>
    /// Payment terms in days
    /// </summary>
    public int PaymentTermsDays { get; set; }

    /// <summary>
    /// Tax ID
    /// </summary>
    public string? TaxId { get; set; }

    /// <summary>
    /// Notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Registration date
    /// </summary>
    public DateTime RegisteredDate { get; set; }

    /// <summary>
    /// Last purchase date
    /// </summary>
    public DateTime? LastPurchaseDate { get; set; }

    /// <summary>
    /// Total purchases amount
    /// </summary>
    public decimal TotalPurchases { get; set; }

    /// <summary>
    /// Available credit
    /// </summary>
    public decimal AvailableCredit => Math.Max(0, CreditLimit - Balance);

    /// <summary>
    /// Is over credit limit
    /// </summary>
    public bool IsOverCreditLimit => Balance > CreditLimit;

    /// <summary>
    /// Created date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Updated date
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
