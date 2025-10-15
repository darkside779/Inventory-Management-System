using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.WebUI.ViewModels.Invoice;

public class InvoiceDetailsViewModel
{
    public int Id { get; set; }
    
    [Display(Name = "Invoice Number")]
    public string InvoiceNumber { get; set; } = string.Empty;
    
    [Display(Name = "Customer")]
    public string CustomerName { get; set; } = string.Empty;
    
    [Display(Name = "Company")]
    public string? CompanyName { get; set; }
    
    [Display(Name = "Customer Email")]
    public string? CustomerEmail { get; set; }
    
    [Display(Name = "Customer Phone")]
    public string? CustomerPhone { get; set; }
    
    [Display(Name = "Invoice Date")]
    public DateTime InvoiceDate { get; set; }
    
    [Display(Name = "Due Date")]
    public DateTime DueDate { get; set; }
    
    [Display(Name = "Payment Terms")]
    public string? PaymentTerms { get; set; }
    
    [Display(Name = "Notes")]
    public string? Notes { get; set; }
    
    [Display(Name = "Status")]
    public string Status { get; set; } = string.Empty;
    
    [Display(Name = "Subtotal")]
    public decimal SubTotal { get; set; }
    
    [Display(Name = "Tax Amount")]
    public decimal TaxAmount { get; set; }
    
    [Display(Name = "Discount Amount")]
    public decimal DiscountAmount { get; set; }
    
    [Display(Name = "Total Amount")]
    public decimal TotalAmount { get; set; }
    
    [Display(Name = "Paid Amount")]
    public decimal PaidAmount { get; set; }
    
    [Display(Name = "Balance")]
    public decimal Balance => TotalAmount - PaidAmount;
    
    [Display(Name = "Created Date")]
    public DateTime CreatedAt { get; set; }
    
    [Display(Name = "Created By")]
    public string? CreatedByUser { get; set; }
    
    public bool IsOverdue => DueDate < DateTime.Now && Balance > 0;
    
    public List<InvoiceItemDetailsViewModel> Items { get; set; } = new();
    
    /// <summary>
    /// Payment history for this invoice
    /// </summary>
    public List<InvoicePaymentViewModel> Payments { get; set; } = new();
}

/// <summary>
/// ViewModel for displaying payment information in invoice details
/// </summary>
public class InvoicePaymentViewModel
{
    public int Id { get; set; }
    
    [Display(Name = "Payment Number")]
    public string PaymentNumber { get; set; } = string.Empty;
    
    [Display(Name = "Payment Date")]
    public DateTime PaymentDate { get; set; }
    
    [Display(Name = "Amount")]
    public decimal Amount { get; set; }
    
    [Display(Name = "Payment Method")]
    public string PaymentMethod { get; set; } = string.Empty;
    
    [Display(Name = "Reference Number")]
    public string? ReferenceNumber { get; set; }
    
    [Display(Name = "Notes")]
    public string? Notes { get; set; }
    
    [Display(Name = "Recorded By")]
    public string RecordedByUser { get; set; } = string.Empty;
}

public class InvoiceItemDetailsViewModel
{
    public int Id { get; set; }
    
    [Display(Name = "Product")]
    public string ProductName { get; set; } = string.Empty;
    
    [Display(Name = "SKU")]
    public string ProductSku { get; set; } = string.Empty;
    
    [Display(Name = "Description")]
    public string? Description { get; set; }
    
    [Display(Name = "Quantity")]
    public int Quantity { get; set; }
    
    [Display(Name = "Unit Price")]
    public decimal UnitPrice { get; set; }
    
    [Display(Name = "Discount %")]
    public decimal DiscountPercentage { get; set; }
    
    [Display(Name = "Tax %")]
    public decimal TaxPercentage { get; set; }
    
    [Display(Name = "Line Total")]
    public decimal LineTotal
    {
        get
        {
            var subtotal = Quantity * UnitPrice;
            var discount = subtotal * (DiscountPercentage / 100);
            var taxable = subtotal - discount;
            var tax = taxable * (TaxPercentage / 100);
            return subtotal - discount + tax;
        }
    }
}
