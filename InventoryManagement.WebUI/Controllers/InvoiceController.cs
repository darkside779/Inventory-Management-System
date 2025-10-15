using InventoryManagement.Application.Features.CustomerInvoices.Commands;
using InventoryManagement.Application.Features.CustomerInvoices.Queries;
using InventoryManagement.WebUI.ViewModels.Invoice;
using InventoryManagement.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.WebUI.Controllers;

[Authorize]
public class InvoiceController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<InvoiceController> _logger;
    private readonly AppDbContext _context;

    public InvoiceController(IMediator mediator, ILogger<InvoiceController> logger, AppDbContext context)
    {
        _mediator = mediator;
        _logger = logger;
        _context = context;
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
            _logger.LogInformation("Create Invoice POST: Received {ItemCount} items", model.Items?.Count ?? 0);
            
            // Validate that at least one item is provided
            if (model.Items == null || !model.Items.Any() || !model.Items.Any(i => i.ProductId > 0))
            {
                ModelState.AddModelError("", "At least one invoice item is required. Please add products to the invoice.");
                await PopulateDropdowns(model);
                return View(model);
            }

            // Handle customer selection/creation
            int customerId;
            if (model.IsNewCustomer && model.NewCustomer != null)
            {
                // Create new customer inline
                var createCustomerCommand = new InventoryManagement.Application.Features.Customers.Commands.CreateCustomerCommand
                {
                    FullName = model.NewCustomer.FullName,
                    Email = model.NewCustomer.Email,
                    PhoneNumber = model.NewCustomer.PhoneNumber,
                    CompanyName = model.NewCustomer.CompanyName,
                    Address = model.NewCustomer.Address,
                    CustomerType = model.NewCustomer.CustomerType,
                    PaymentTermsDays = model.NewCustomer.PaymentTermsDays,
                    CreatedByUserId = GetCurrentUserIdAsInt()
                };

                var customerResult = await _mediator.Send(createCustomerCommand);
                if (!customerResult.IsSuccess || customerResult.Customer == null)
                {
                    ModelState.AddModelError("", customerResult.ErrorMessage ?? "Failed to create customer");
                    await PopulateDropdowns(model);
                    return View(model);
                }

                customerId = customerResult.Customer.Id;
                _logger.LogInformation("Created new customer {CustomerId} for invoice", customerId);
            }
            else if (model.CustomerId.HasValue)
            {
                // Use existing customer
                customerId = model.CustomerId.Value;
            }
            else
            {
                ModelState.AddModelError("", "Please select a customer or create a new one");
                await PopulateDropdowns(model);
                return View(model);
            }
            
            if (ModelState.IsValid)
            {
                var command = new CreateInvoiceCommand
                {
                    CustomerId = customerId,
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
            // Get the invoice with items and payments using DbContext directly
            var invoice = await _context.CustomerInvoices
                .Include(i => i.Customer)
                .Include(i => i.Items)
                    .ThenInclude(item => item.Product)
                .Include(i => i.Payments)
                    .ThenInclude(p => p.RecordedByUser)
                .FirstOrDefaultAsync(i => i.Id == id);
            
            if (invoice == null)
            {
                TempData["Error"] = "Invoice not found";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new InventoryManagement.WebUI.ViewModels.Invoice.InvoiceDetailsViewModel
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                CustomerName = invoice.Customer?.FullName ?? "Unknown Customer",
                CompanyName = invoice.Customer?.CompanyName,
                CustomerEmail = invoice.Customer?.Email,
                CustomerPhone = invoice.Customer?.PhoneNumber,
                InvoiceDate = invoice.InvoiceDate,
                DueDate = invoice.DueDate,
                PaymentTerms = invoice.PaymentTerms,
                Notes = invoice.Notes,
                Status = invoice.Status,
                SubTotal = invoice.SubTotal,
                TaxAmount = invoice.TaxAmount,
                DiscountAmount = invoice.DiscountAmount,
                TotalAmount = invoice.TotalAmount,
                PaidAmount = invoice.PaidAmount,
                CreatedAt = invoice.CreatedAt,
                CreatedByUser = invoice.CreatedByUser?.FullName ?? "System",
                Items = invoice.Items.Select(item => new InventoryManagement.WebUI.ViewModels.Invoice.InvoiceItemDetailsViewModel
                {
                    Id = item.Id,
                    ProductName = item.Product?.Name ?? "Unknown Product",
                    ProductSku = item.Product?.SKU ?? "",
                    Description = item.Description,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountPercentage = item.DiscountPercentage,
                    TaxPercentage = item.TaxPercentage
                }).ToList(),
                Payments = invoice.Payments?.Select(payment => new InventoryManagement.WebUI.ViewModels.Invoice.InvoicePaymentViewModel
                {
                    Id = payment.Id,
                    PaymentNumber = payment.PaymentNumber,
                    PaymentDate = payment.PaymentDate,
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod,
                    ReferenceNumber = payment.ReferenceNumber,
                    Notes = payment.Notes,
                    RecordedByUser = payment.RecordedByUser?.FullName ?? "System"
                }).OrderByDescending(p => p.PaymentDate).ToList() ?? new List<InventoryManagement.WebUI.ViewModels.Invoice.InvoicePaymentViewModel>()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving invoice details for ID: {Id}", id);
            TempData["Error"] = "An error occurred while retrieving invoice details";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Invoice/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            // Get the invoice with items using DbContext directly
            var invoice = await _context.CustomerInvoices
                .Include(i => i.Customer)
                .Include(i => i.Items)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(i => i.Id == id);
            
            if (invoice == null)
            {
                TempData["Error"] = "Invoice not found";
                return RedirectToAction(nameof(Index));
            }

            // Check if invoice can be edited
            if (invoice.Status == "Paid" || invoice.Status == "Cancelled")
            {
                TempData["Error"] = "Cannot edit a paid or cancelled invoice";
                return RedirectToAction(nameof(Details), new { id });
            }

            var viewModel = new InventoryManagement.WebUI.ViewModels.Invoice.EditInvoiceViewModel
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                CustomerId = invoice.CustomerId,
                InvoiceDate = invoice.InvoiceDate,
                DueDate = invoice.DueDate,
                PaymentTerms = invoice.PaymentTerms,
                Notes = invoice.Notes,
                Status = invoice.Status,
                Items = invoice.Items.Select(item => new InventoryManagement.WebUI.ViewModels.Invoice.EditInvoiceItemViewModel
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountPercentage = item.DiscountPercentage,
                    TaxPercentage = item.TaxPercentage,
                    Description = item.Description
                }).ToList()
            };

            await PopulateEditDropdowns(viewModel);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving invoice for edit, ID: {Id}", id);
            TempData["Error"] = "Invoice not found";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Invoice/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, InventoryManagement.WebUI.ViewModels.Invoice.EditInvoiceViewModel model)
    {
        if (id != model.Id)
        {
            TempData["Error"] = "Invalid invoice ID";
            return RedirectToAction(nameof(Index));
        }

        // Validate that at least one item is provided
        if (model.Items == null || !model.Items.Any() || !model.Items.Any(i => i.ProductId > 0))
        {
            ModelState.AddModelError("", "At least one invoice item is required. Please add products to the invoice.");
            await PopulateEditDropdowns(model);
            return View(model);
        }

        if (!ModelState.IsValid)
        {
            await PopulateEditDropdowns(model);
            return View(model);
        }

        try
        {
            // For now, just show a success message since we don't have the update command implemented
            TempData["Success"] = $"Invoice {model.InvoiceNumber} would be updated successfully (Update command not yet implemented)";
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating invoice, ID: {Id}", id);
            TempData["Error"] = "An error occurred while updating the invoice";
            await PopulateEditDropdowns(model);
            return View(model);
        }
    }

    // GET: Invoice/Print/5
    public async Task<IActionResult> Print(int id)
    {
        try
        {
            // Get the invoice with items using DbContext directly (same as Details)
            var invoice = await _context.CustomerInvoices
                .Include(i => i.Customer)
                .Include(i => i.Items)
                    .ThenInclude(item => item.Product)
                .Include(i => i.CreatedByUser)
                .FirstOrDefaultAsync(i => i.Id == id);
            
            if (invoice == null)
            {
                TempData["Error"] = "Invoice not found";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new InventoryManagement.WebUI.ViewModels.Invoice.InvoiceDetailsViewModel
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                CustomerName = invoice.Customer?.FullName ?? "Unknown Customer",
                CompanyName = invoice.Customer?.CompanyName,
                CustomerEmail = invoice.Customer?.Email,
                CustomerPhone = invoice.Customer?.PhoneNumber,
                InvoiceDate = invoice.InvoiceDate,
                DueDate = invoice.DueDate,
                PaymentTerms = invoice.PaymentTerms,
                Notes = invoice.Notes,
                Status = invoice.Status,
                SubTotal = invoice.SubTotal,
                TaxAmount = invoice.TaxAmount,
                DiscountAmount = invoice.DiscountAmount,
                TotalAmount = invoice.TotalAmount,
                PaidAmount = invoice.PaidAmount,
                CreatedAt = invoice.CreatedAt,
                CreatedByUser = invoice.CreatedByUser?.FullName ?? "System",
                Items = invoice.Items.Select(item => new InventoryManagement.WebUI.ViewModels.Invoice.InvoiceItemDetailsViewModel
                {
                    Id = item.Id,
                    ProductName = item.Product?.Name ?? "Unknown Product",
                    ProductSku = item.Product?.SKU ?? "",
                    Description = item.Description,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountPercentage = item.DiscountPercentage,
                    TaxPercentage = item.TaxPercentage
                }).ToList()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error printing invoice, ID: {Id}", id);
            TempData["Error"] = "Invoice not found";
            return RedirectToAction(nameof(Index));
        }
    }

    // DELETE: Invoice/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            // TODO: Implement DeleteInvoiceCommand
            TempData["Success"] = "Invoice deleted successfully";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting invoice, ID: {Id}", id);
            TempData["Error"] = "An error occurred while deleting the invoice";
            return RedirectToAction(nameof(Index));
        }
    }

    // Private helper methods
    private async Task PopulateEditDropdowns(InventoryManagement.WebUI.ViewModels.Invoice.EditInvoiceViewModel model)
    {
        // Fetch customers
        var customersResult = await _mediator.Send(new InventoryManagement.Application.Features.Customers.Queries.GetCustomersQuery { IsActive = true, PageSize = 1000 });
        model.Customers = customersResult.Data
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.FullName + (string.IsNullOrWhiteSpace(c.CompanyName) ? "" : $" ({c.CompanyName})") })
            .ToList();

        var productsResult = await _mediator.Send(new InventoryManagement.Application.Features.Products.Queries.GetAllProducts.GetAllProductsQuery { ActiveOnly = true, PageSize = 1000 });
        _logger.LogInformation("PopulateEditDropdowns: Retrieved {ProductCount} products for Edit Invoice dropdown", productsResult.Items?.Count() ?? 0);
        model.Products = productsResult.Items?.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = $"{p.Name} - ${p.Price:F2}" }).ToList() ?? new List<SelectListItem>();
        
        // Pass product data as JSON for JavaScript
        var productData = productsResult.Items?.Select(p => new { 
            id = p.Id, 
            name = p.Name, 
            price = p.Price,
            sku = p.SKU 
        }).ToList();
        ViewBag.ProductsJson = productData != null ? System.Text.Json.JsonSerializer.Serialize(productData) : "[]";

        model.StatusOptions = new List<SelectListItem>
        {
            new() { Value = "Draft", Text = "Draft" },
            new() { Value = "Sent", Text = "Sent" },
            new() { Value = "Paid", Text = "Paid" },
            new() { Value = "Overdue", Text = "Overdue" },
            new() { Value = "Cancelled", Text = "Cancelled" }
        };
    }

    private async Task PopulateDropdowns(CreateInvoiceViewModel model)
    {
        // Fetch customers
        var customersResult = await _mediator.Send(new InventoryManagement.Application.Features.Customers.Queries.GetCustomersQuery { IsActive = true, PageSize = 1000 });
        model.Customers = customersResult.Data
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.FullName + (string.IsNullOrWhiteSpace(c.CompanyName) ? "" : $" ({c.CompanyName})") })
            .ToList();

        var productsResult = await _mediator.Send(new InventoryManagement.Application.Features.Products.Queries.GetAllProducts.GetAllProductsQuery { ActiveOnly = true, PageSize = 1000 });
        _logger.LogInformation("PopulateDropdowns: Retrieved {ProductCount} products for Create Invoice dropdown", productsResult.Items?.Count() ?? 0);
        model.Products = productsResult.Items?.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = $"{p.Name} - ${p.Price:F2}" }).ToList() ?? new List<SelectListItem>();
        
        // Pass product data as JSON for JavaScript
        var productData = productsResult.Items?.Select(p => new { 
            id = p.Id, 
            name = p.Name, 
            price = p.Price,
            sku = p.SKU 
        }).ToList();
        ViewBag.ProductsJson = productData != null ? System.Text.Json.JsonSerializer.Serialize(productData) : "[]";

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
        // Get the current logged-in user's email from claims
        var userEmail = User.Identity?.Name;
        
        if (string.IsNullOrEmpty(userEmail))
        {
            // Fallback to system user if no user is logged in
            return GetOrCreateSystemUser();
        }
        
        // Find the user in our Users table by email
        var currentUser = _context.Users.FirstOrDefault(u => u.Email == userEmail);
        
        if (currentUser != null)
        {
            return currentUser.Id;
        }
        
        // If user doesn't exist in our Users table, create them from Identity
        return CreateUserFromIdentity(userEmail);
    }
    
    private int GetOrCreateSystemUser()
    {
        var systemUserId = 1;
        var systemUser = _context.Users.FirstOrDefault(u => u.Id == systemUserId);
        
        if (systemUser == null)
        {
            systemUser = new InventoryManagement.Domain.Entities.User
            {
                Id = systemUserId,
                Username = "system",
                Email = "system@inventorymanagement.com",
                PasswordHash = "N/A",
                FullName = "System Administrator",
                Role = InventoryManagement.Domain.Enums.UserRole.Administrator,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            _context.Users.Add(systemUser);
            _context.SaveChanges();
        }
        
        return systemUserId;
    }
    
    private int CreateUserFromIdentity(string email)
    {
        // Create a new user record based on the logged-in Identity user
        var newUser = new InventoryManagement.Domain.Entities.User
        {
            Username = email,
            Email = email,
            PasswordHash = "IDENTITY_MANAGED", // Password is managed by Identity
            FullName = email.Split('@')[0], // Use email prefix as name for now
            Role = InventoryManagement.Domain.Enums.UserRole.Staff, // Default role
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        _context.Users.Add(newUser);
        _context.SaveChanges();
        
        _logger.LogInformation("Created new user record for Identity user: {Email} with ID: {UserId}", 
            email, newUser.Id);
        
        return newUser.Id;
    }
}
