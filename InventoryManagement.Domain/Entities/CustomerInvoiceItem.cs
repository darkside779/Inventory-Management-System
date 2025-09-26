using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Entities;

/// <summary>
/// Customer invoice line item
/// </summary>
public class CustomerInvoiceItem : BaseEntity
{
    /// <summary>
    /// Item unique identifier
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Invoice ID
    /// </summary>
    [ForeignKey(nameof(Invoice))]
    public int InvoiceId { get; set; }

    /// <summary>
    /// Product ID
    /// </summary>
    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }

    /// <summary>
    /// Quantity sold
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Unit price
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Discount percentage on this item
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal DiscountPercentage { get; set; } = 0;

    /// <summary>
    /// Tax percentage on this item
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal TaxPercentage { get; set; } = 0;

    /// <summary>
    /// Line total (quantity * unit price - discount + tax)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal LineTotal 
    { 
        get 
        {
            var subtotal = Quantity * UnitPrice;
            var discount = subtotal * (DiscountPercentage / 100);
            var afterDiscount = subtotal - discount;
            var tax = afterDiscount * (TaxPercentage / 100);
            return afterDiscount + tax;
        } 
    }

    /// <summary>
    /// Item description/notes
    /// </summary>
    [StringLength(200)]
    public string? Description { get; set; }

    // Navigation Properties

    /// <summary>
    /// Invoice
    /// </summary>
    public virtual CustomerInvoice Invoice { get; set; } = null!;

    /// <summary>
    /// Product
    /// </summary>
    public virtual Product Product { get; set; } = null!;
}
