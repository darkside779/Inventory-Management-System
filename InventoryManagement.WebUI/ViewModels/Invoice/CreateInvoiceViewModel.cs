using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.WebUI.ViewModels.Invoice;

/// <summary>
/// ViewModel for creating a new invoice
/// </summary>
public class CreateInvoiceViewModel
{
    /// <summary>
    /// Customer ID
    /// </summary>
    [Required]
    [Display(Name = "Customer")]
    public int CustomerId { get; set; }

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
