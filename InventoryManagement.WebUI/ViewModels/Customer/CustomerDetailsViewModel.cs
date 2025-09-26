using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.WebUI.ViewModels.Customer;

/// <summary>
/// View model for customer details
/// </summary>
public class CustomerDetailsViewModel
{
    /// <summary>
    /// Customer ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Customer code
    /// </summary>
    [Display(Name = "Customer Code")]
    public string CustomerCode { get; set; } = string.Empty;

    /// <summary>
    /// Full name
    /// </summary>
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Company name
    /// </summary>
    [Display(Name = "Company Name")]
    public string? CompanyName { get; set; }

    /// <summary>
    /// Email address
    /// </summary>
    [Display(Name = "Email")]
    public string? Email { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Address
    /// </summary>
    [Display(Name = "Address")]
    public string? Address { get; set; }

    /// <summary>
    /// Customer type
    /// </summary>
    [Display(Name = "Customer Type")]
    public string CustomerType { get; set; } = string.Empty;

    /// <summary>
    /// Current balance
    /// </summary>
    [Display(Name = "Balance")]
    public decimal Balance { get; set; }

    /// <summary>
    /// Credit limit
    /// </summary>
    [Display(Name = "Credit Limit")]
    public decimal CreditLimit { get; set; }

    /// <summary>
    /// Payment terms in days
    /// </summary>
    [Display(Name = "Payment Terms (Days)")]
    public int PaymentTermsDays { get; set; }

    /// <summary>
    /// Tax ID
    /// </summary>
    [Display(Name = "Tax ID")]
    public string? TaxId { get; set; }

    /// <summary>
    /// Notes
    /// </summary>
    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    /// <summary>
    /// Is active
    /// </summary>
    [Display(Name = "Active")]
    public bool IsActive { get; set; }

    /// <summary>
    /// Registration date
    /// </summary>
    [Display(Name = "Registered Date")]
    public DateTime RegisteredDate { get; set; }

    /// <summary>
    /// Last purchase date
    /// </summary>
    [Display(Name = "Last Purchase")]
    public DateTime? LastPurchaseDate { get; set; }

    /// <summary>
    /// Total purchases amount
    /// </summary>
    [Display(Name = "Total Purchases")]
    public decimal TotalPurchases { get; set; }

    /// <summary>
    /// Available credit
    /// </summary>
    [Display(Name = "Available Credit")]
    public decimal AvailableCredit => Math.Max(0, CreditLimit - Balance);

    /// <summary>
    /// Is over credit limit
    /// </summary>
    public bool IsOverCreditLimit => Balance > CreditLimit;
}
