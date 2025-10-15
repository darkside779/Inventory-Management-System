using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.ViewModels.Invoice;

public class EditInvoiceViewModel
{
    public int Id { get; set; }
    
    [Required]
    [Display(Name = "Invoice Number")]
    public string InvoiceNumber { get; set; } = string.Empty;
    
    [Required]
    [Display(Name = "Customer")]
    public int CustomerId { get; set; }
    
    [Required]
    [Display(Name = "Invoice Date")]
    [DataType(DataType.Date)]
    public DateTime InvoiceDate { get; set; } = DateTime.Today;
    
    [Required]
    [Display(Name = "Due Date")]
    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(30);
    
    [Display(Name = "Payment Terms")]
    public string? PaymentTerms { get; set; }
    
    [Display(Name = "Notes")]
    public string? Notes { get; set; }
    
    [Required]
    [Display(Name = "Status")]
    public string Status { get; set; } = "Draft";
    
    // Dropdown data
    public IEnumerable<SelectListItem> Customers { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Products { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();
    
    // Invoice items
    public List<EditInvoiceItemViewModel> Items { get; set; } = new();
    
    // Calculated fields
    public decimal SubTotal => Items?.Sum(i => i.LineTotal) ?? 0;
    public decimal TaxAmount => Items?.Sum(i => i.TaxAmount) ?? 0;
    public decimal DiscountAmount => Items?.Sum(i => i.DiscountAmount) ?? 0;
    public decimal TotalAmount => SubTotal + TaxAmount - DiscountAmount;
}

public class EditInvoiceItemViewModel
{
    public int Id { get; set; }
    
    [Required]
    [Display(Name = "Product")]
    public int ProductId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    [Display(Name = "Quantity")]
    public int Quantity { get; set; } = 1;
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
    [Display(Name = "Unit Price")]
    public decimal UnitPrice { get; set; }
    
    [Range(0, 100, ErrorMessage = "Discount percentage must be between 0 and 100")]
    [Display(Name = "Discount %")]
    public decimal DiscountPercentage { get; set; } = 0;
    
    [Range(0, 100, ErrorMessage = "Tax percentage must be between 0 and 100")]
    [Display(Name = "Tax %")]
    public decimal TaxPercentage { get; set; } = 0;
    
    [Display(Name = "Description")]
    public string? Description { get; set; }
    
    // Calculated fields
    public decimal LineSubtotal => Quantity * UnitPrice;
    public decimal DiscountAmount => LineSubtotal * (DiscountPercentage / 100);
    public decimal TaxableAmount => LineSubtotal - DiscountAmount;
    public decimal TaxAmount => TaxableAmount * (TaxPercentage / 100);
    public decimal LineTotal => LineSubtotal - DiscountAmount + TaxAmount;
    
    // For display
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
}
