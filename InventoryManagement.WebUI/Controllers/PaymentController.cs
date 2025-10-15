using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using InventoryManagement.WebUI.ViewModels.Payment;
using InventoryManagement.Application.Features.Payments.Commands.CreatePayment;
using InventoryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.WebUI.Controllers;

[Authorize]
public class PaymentController : Controller
{
    private readonly ILogger<PaymentController> _logger;
    private readonly IMediator _mediator;
    private readonly AppDbContext _context;

    public PaymentController(ILogger<PaymentController> logger, IMediator mediator, AppDbContext context)
    {
        _logger = logger;
        _mediator = mediator;
        _context = context;
    }

    // GET: Payment/Create?invoiceId=5
    public async Task<IActionResult> Create(int invoiceId)
    {
        try
        {
            // Get invoice with payments
            var invoice = await _context.CustomerInvoices
                .Include(i => i.Customer)
                .Include(i => i.Payments)
                    .ThenInclude(p => p.RecordedByUser)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);

            if (invoice == null)
            {
                TempData["Error"] = "Invoice not found";
                return RedirectToAction("Index", "Invoice");
            }

            if (invoice.Status == "Cancelled")
            {
                TempData["Error"] = "Cannot record payment for cancelled invoice";
                return RedirectToAction("Details", "Invoice", new { id = invoiceId });
            }

            var paidAmount = invoice.Payments?.Sum(p => p.Amount) ?? 0;
            var remainingBalance = invoice.TotalAmount - paidAmount;

            if (remainingBalance <= 0)
            {
                TempData["Info"] = "Invoice is already fully paid";
                return RedirectToAction("Details", "Invoice", new { id = invoiceId });
            }

            var viewModel = new CreatePaymentViewModel
            {
                InvoiceId = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                CustomerName = invoice.Customer?.FullName ?? "Unknown Customer",
                InvoiceTotal = invoice.TotalAmount,
                PaidAmount = paidAmount,
                Amount = remainingBalance, // Default to full remaining amount
                PaymentHistory = invoice.Payments?.Select(p => new PaymentHistoryViewModel
                {
                    Id = p.Id,
                    PaymentNumber = p.PaymentNumber,
                    PaymentDate = p.PaymentDate,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    ReferenceNumber = p.ReferenceNumber,
                    Notes = p.Notes,
                    RecordedByUser = p.RecordedByUser?.FullName ?? "System"
                }).OrderByDescending(p => p.PaymentDate).ToList() ?? new List<PaymentHistoryViewModel>()
            };

            PopulatePaymentMethods(viewModel);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment for invoice ID: {InvoiceId}", invoiceId);
            TempData["Error"] = "An error occurred while accessing payment functionality";
            return RedirectToAction("Index", "Invoice");
        }
    }

    // POST: Payment/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePaymentViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                PopulatePaymentMethods(model);
                return View(model);
            }

            var command = new CreatePaymentCommand
            {
                InvoiceId = model.InvoiceId,
                Amount = model.Amount,
                PaymentDate = model.PaymentDate,
                PaymentMethod = model.PaymentMethod,
                ReferenceNumber = model.ReferenceNumber,
                Notes = model.Notes,
                RecordedByUserId = GetCurrentUserIdAsInt()
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                TempData["Success"] = $"Payment {result.Payment?.PaymentNumber} recorded successfully. Remaining balance: ${result.InvoiceSummary?.RemainingBalance:F2}";
                return RedirectToAction("Details", "Invoice", new { id = model.InvoiceId });
            }
            else
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "Failed to record payment");
                PopulatePaymentMethods(model);
                return View(model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording payment");
            TempData["Error"] = "An error occurred while recording the payment";
            PopulatePaymentMethods(model);
            return View(model);
        }
    }

    // GET: Payment/Index
    public async Task<IActionResult> Index()
    {
        try
        {
            // TODO: Implement payment listing functionality
            TempData["Info"] = "Payment management - implementation pending";
            return RedirectToAction("Index", "Invoice");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments");
            TempData["Error"] = "An error occurred while retrieving payments";
            return RedirectToAction("Index", "Invoice");
        }
    }

    // Helper methods
    private void PopulatePaymentMethods(CreatePaymentViewModel model)
    {
        model.PaymentMethods = new List<SelectListItem>
        {
            new() { Value = "Cash", Text = "Cash" },
            new() { Value = "Credit Card", Text = "Credit Card" },
            new() { Value = "Debit Card", Text = "Debit Card" },
            new() { Value = "Bank Transfer", Text = "Bank Transfer" },
            new() { Value = "Check", Text = "Check" },
            new() { Value = "Online Payment", Text = "Online Payment" },
            new() { Value = "Other", Text = "Other" }
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
