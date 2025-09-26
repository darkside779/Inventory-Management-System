using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.ViewModels.Customer;

/// <summary>
/// View model for creating a new customer
/// </summary>
public class CreateCustomerViewModel
{
    /// <summary>
    /// Full name
    /// </summary>
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Company name
    /// </summary>
    [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
    [Display(Name = "Company Name")]
    public string? CompanyName { get; set; }

    /// <summary>
    /// Email address
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Address
    /// </summary>
    [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    [Display(Name = "Address")]
    public string? Address { get; set; }

    /// <summary>
    /// Customer type
    /// </summary>
    [Required(ErrorMessage = "Customer type is required")]
    [Display(Name = "Customer Type")]
    public string CustomerType { get; set; } = "Individual";

    /// <summary>
    /// Credit limit
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Credit limit must be 0 or greater")]
    [Display(Name = "Credit Limit")]
    public decimal CreditLimit { get; set; } = 0;

    /// <summary>
    /// Payment terms in days
    /// </summary>
    [Range(0, 365, ErrorMessage = "Payment terms must be between 0 and 365 days")]
    [Display(Name = "Payment Terms (Days)")]
    public int PaymentTermsDays { get; set; } = 30;

    /// <summary>
    /// Tax ID
    /// </summary>
    [StringLength(50, ErrorMessage = "Tax ID cannot exceed 50 characters")]
    [Display(Name = "Tax ID")]
    public string? TaxId { get; set; }

    /// <summary>
    /// Notes
    /// </summary>
    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    // Dropdown lists
    /// <summary>
    /// Customer type options
    /// </summary>
    public SelectList? CustomerTypes { get; set; }

    /// <summary>
    /// Payment terms options
    /// </summary>
    public SelectList? PaymentTermsOptions { get; set; }
}
