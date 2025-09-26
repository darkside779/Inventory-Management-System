using InventoryManagement.Application.Features.CustomerInvoices.Commands;
using InventoryManagement.Application.Features.CustomerInvoices.Queries;
using InventoryManagement.WebUI.ViewModels.Invoice;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.Controllers;

[Authorize]
public class InvoiceController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<InvoiceController> _logger;

    public InvoiceController(IMediator mediator, ILogger<InvoiceController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    // GET: Invoice
    public async Task<IActionResult> Index(InvoiceIndexViewModel filter)
    {
        try
        {
            var query = new GetInvoicesQuery
            {
                SearchTerm = filter.SearchTerm,
                CustomerId = filter.CustomerId,
                Status = filter.Status,
                InvoiceDateFrom = filter.InvoiceDateFrom,
                InvoiceDateTo = filter.InvoiceDateTo,
                DueDateFrom = filter.DueDateFrom,
                DueDateTo = filter.DueDateTo,
                IsOverdue = filter.IsOverdue,
                Page = filter.Page > 0 ? filter.Page : 1,
                PageSize = filter.PageSize > 0 ? filter.PageSize : 10,
                SortBy = filter.SortBy ?? "InvoiceDate",
                SortDirection = filter.SortDirection ?? "desc"
            };

            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                var viewModel = new InvoiceIndexViewModel
                {
                    Invoices = result.Data,
                    TotalCount = result.TotalCount,
                    Page = query.Page,
                    PageSize = query.PageSize,
                    SearchTerm = query.SearchTerm,
                    CustomerId = query.CustomerId,
                    Status = query.Status,
                    InvoiceDateFrom = query.InvoiceDateFrom,
                    InvoiceDateTo = query.InvoiceDateTo,
                    DueDateFrom = query.DueDateFrom,
                    DueDateTo = query.DueDateTo,
                    IsOverdue = query.IsOverdue,
                    SortBy = query.SortBy,
                    SortDirection = query.SortDirection
                };

                return View(viewModel);
            }
            else
            {
                TempData["Error"] = result.ErrorMessage ?? "An error occurred while retrieving invoices";
                return View(new InvoiceIndexViewModel());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Invoice Index");
            TempData["Error"] = "An error occurred while retrieving invoices";
            return View(new InvoiceIndexViewModel());
        }
    }

    // GET: Invoice/Create
    public async Task<IActionResult> Create(int? customerId)
    {
        try
        {
            var viewModel = new CreateInvoiceViewModel();
            
            if (customerId.HasValue)
            {
                viewModel.CustomerId = customerId.Value;
            }

            await PopulateDropdowns(viewModel);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading invoice create page");
            TempData["Error"] = "An error occurred while loading the page";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Invoice/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateInvoiceViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var command = new CreateInvoiceCommand
                {
                    CustomerId = model.CustomerId,
                    InvoiceDate = model.InvoiceDate,
                    DueDate = model.DueDate,
                    PaymentTerms = model.PaymentTerms,
                    Notes = model.Notes,
                    Items = model.Items.Select(i => new CreateInvoiceItemDto
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        DiscountPercentage = i.DiscountPercentage,
                        TaxPercentage = i.TaxPercentage,
                        Description = i.Description
                    }).ToList(),
                    CreatedByUserId = GetCurrentUserIdAsInt()
                };

                var result = await _mediator.Send(command);

                if (result.IsSuccess)
                {
                    TempData["Success"] = "Invoice created successfully";
                    return RedirectToAction(nameof(Details), new { id = result.Invoice!.Id });
                }
                else
                {
                    ModelState.AddModelError("", result.ErrorMessage ?? "Failed to create invoice");
                }
            }

            await PopulateDropdowns(model);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invoice");
            ModelState.AddModelError("", "An error occurred while creating the invoice");
            await PopulateDropdowns(model);
            return View(model);
        }
    }

    // GET: Invoice/Details/5
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            // We'll implement GetInvoiceByIdQuery later
            TempData["Info"] = "Invoice details view - implementation pending";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving invoice details for ID: {Id}", id);
            TempData["Error"] = "Invoice not found";
            return RedirectToAction(nameof(Index));
        }
    }

    // Private helper methods
    private async Task PopulateDropdowns(CreateInvoiceViewModel model)
    {
        // We'll implement these when we have the necessary queries
        model.Customers = new List<SelectListItem>();
        model.Products = new List<SelectListItem>();
        model.StatusOptions = new List<SelectListItem>
        {
            new() { Value = "Draft", Text = "Draft" },
            new() { Value = "Sent", Text = "Sent" },
            new() { Value = "Paid", Text = "Paid" },
            new() { Value = "Overdue", Text = "Overdue" },
            new() { Value = "Cancelled", Text = "Cancelled" }
        };
    }

    private int GetCurrentUserIdAsInt()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }
        
        // For development, return a default user ID
        // In production, you'd look up the user ID from the Identity system
        return 1; // Temporary - should be replaced with actual user lookup
    }
}
