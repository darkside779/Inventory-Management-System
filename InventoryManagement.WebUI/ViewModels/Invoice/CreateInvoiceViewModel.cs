using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.WebUI.ViewModels.Invoice;

/// <summary>
/// ViewModel for creating a new invoice
/// </summary>
public class CreateInvoiceViewModel
{
    /// <summary>
    /// Customer ID (for existing customers)
    /// </summary>
    [Display(Name = "Customer")]
    public int? CustomerId { get; set; }

    /// <summary>
    /// Customer search text (for autocomplete)
    /// </summary>
    [Display(Name = "Customer")]
    public string? CustomerSearchText { get; set; }

    /// <summary>
    /// Flag to indicate if creating a new customer inline
    /// </summary>
    public bool IsNewCustomer { get; set; }

    /// <summary>
    /// New customer details (for inline creation)
    /// </summary>
    public InlineCustomerViewModel? NewCustomer { get; set; }

    /// <summary>
    /// Invoice date
    /// </summary>
    [Required]
    [Display(Name = "Invoice Date")]
    public DateTime InvoiceDate { get; set; } = DateTime.Today;

    /// <summary>
    /// Due date
    /// </summary>
    [Required]
    [Display(Name = "Due Date")]
    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(30);

    /// <summary>
    /// Payment terms
    /// </summary>
    [Display(Name = "Payment Terms")]
    public string? PaymentTerms { get; set; }

    /// <summary>
    /// Notes
    /// </summary>
    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    /// <summary>
    /// Invoice items
    /// </summary>
    public List<CreateInvoiceItemViewModel> Items { get; set; } = new();

    // Dropdowns
    /// <summary>
    /// Available customers
    /// </summary>
    public IEnumerable<SelectListItem> Customers { get; set; } = new List<SelectListItem>();

    /// <summary>
    /// Available products
    /// </summary>
    public IEnumerable<SelectListItem> Products { get; set; } = new List<SelectListItem>();

    /// <summary>
    /// Status options
    /// </summary>
    public IEnumerable<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();
}

/// <summary>
/// ViewModel for invoice line items
/// </summary>
public class CreateInvoiceItemViewModel
{
    /// <summary>
    /// Product ID
    /// </summary>
    [Required]
    public int ProductId { get; set; }

    /// <summary>
    /// Product name (for display)
    /// </summary>
    public string? ProductName { get; set; }

    /// <summary>
    /// Quantity
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Unit price
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
    [Display(Name = "Unit Price")]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Discount percentage
    /// </summary>
    [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100%")]
    [Display(Name = "Discount %")]
    public decimal DiscountPercentage { get; set; } = 0;

    /// <summary>
    /// Tax percentage
    /// </summary>
    [Range(0, 100, ErrorMessage = "Tax must be between 0 and 100%")]
    [Display(Name = "Tax %")]
    public decimal TaxPercentage { get; set; } = 0;

    /// <summary>
    /// Description
    /// </summary>
    [Display(Name = "Description")]
    public string? Description { get; set; }

    /// <summary>
    /// Line total (calculated)
    /// </summary>
    public decimal LineTotal => Quantity * UnitPrice;

    /// <summary>
    /// Discount amount (calculated)
    /// </summary>
    public decimal DiscountAmount => LineTotal * (DiscountPercentage / 100);

    /// <summary>
    /// Tax amount (calculated)
    /// </summary>
    public decimal TaxAmount => (LineTotal - DiscountAmount) * (TaxPercentage / 100);

    /// <summary>
    /// Final total (calculated)
    /// </summary>
    public decimal FinalTotal => LineTotal - DiscountAmount + TaxAmount;
}

/// <summary>
/// ViewModel for inline customer creation
/// </summary>
public class InlineCustomerViewModel
{
    /// <summary>
    /// Full name
    /// </summary>
    [Required(ErrorMessage = "Customer name is required")]
    [Display(Name = "Full Name")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Email address
    /// </summary>
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [Display(Name = "Email")]
    [StringLength(100)]
    public string? Email { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    [Required(ErrorMessage = "Phone number is required")]
    [Display(Name = "Phone")]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Company name (optional)
    /// </summary>
    [Display(Name = "Company")]
    [StringLength(100)]
    public string? CompanyName { get; set; }

    /// <summary>
    /// Address (optional)
    /// </summary>
    [Display(Name = "Address")]
    [StringLength(500)]
    public string? Address { get; set; }

    /// <summary>
    /// Customer type
    /// </summary>
    [Display(Name = "Customer Type")]
    public string CustomerType { get; set; } = "Individual";

    /// <summary>
    /// Payment terms in days
    /// </summary>
    [Display(Name = "Payment Terms (Days)")]
    public int PaymentTermsDays { get; set; } = 30;
}
