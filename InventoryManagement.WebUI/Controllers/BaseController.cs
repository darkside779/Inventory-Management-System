using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Security.Claims;

namespace InventoryManagement.WebUI.Controllers;

/// <summary>
/// Base controller providing common functionality for all controllers
/// </summary>
[Authorize]
public abstract class BaseController : Controller
{
    protected readonly IMediator _mediator;
    protected readonly ILogger _logger;

    protected BaseController(IMediator mediator, ILogger logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get current user ID from claims
    /// </summary>
    protected string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }

    /// <summary>
    /// Get current user ID as integer from claims
    /// </summary>
    protected int GetCurrentUserIdAsInt()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userId, out var id) ? id : 1; // Default to user ID 1 if parsing fails
    }

    /// <summary>
    /// Get current user email from claims
    /// </summary>
    protected string GetCurrentUserEmail()
    {
        return User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? string.Empty;
    }

    /// <summary>
    /// Check if current user has specific role
    /// </summary>
    protected bool IsInRole(string role)
    {
        return User.IsInRole(role);
    }

    /// <summary>
    /// Check if current user is admin
    /// </summary>
    protected bool IsAdmin => IsInRole("Admin");

    /// <summary>
    /// Check if current user is manager or admin
    /// </summary>
    protected bool IsManagerOrAdmin => IsInRole("Admin") || IsInRole("Manager");

    /// <summary>
    /// Set success notification message
    /// </summary>
    protected void SetSuccessMessage(string message)
    {
        TempData["SuccessMessage"] = message;
    }

    /// <summary>
    /// Set error notification message
    /// </summary>
    protected void SetErrorMessage(string message)
    {
        TempData["ErrorMessage"] = message;
    }

    /// <summary>
    /// Set warning notification message
    /// </summary>
    protected void SetWarningMessage(string message)
    {
        TempData["WarningMessage"] = message;
    }

    /// <summary>
    /// Set info notification message
    /// </summary>
    protected void SetInfoMessage(string message)
    {
        TempData["InfoMessage"] = message;
    }

    /// <summary>
    /// Handle exceptions and return appropriate error view
    /// </summary>
    protected IActionResult HandleException(Exception ex, string operation = "operation")
    {
        _logger.LogError(ex, "Error occurred during {Operation} by user {UserId}", operation, GetCurrentUserId());
        
        SetErrorMessage($"An error occurred while performing the {operation}. Please try again.");
        
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(new { success = false, message = $"An error occurred while performing the {operation}." });
        }

        return View("Error");
    }

    /// <summary>
    /// Return JSON result for AJAX requests
    /// </summary>
    protected IActionResult JsonSuccess(string message, object? data = null)
    {
        return Json(new { success = true, message, data });
    }

    /// <summary>
    /// Return JSON error result for AJAX requests
    /// </summary>
    protected IActionResult JsonError(string message, object? errors = null)
    {
        return Json(new { success = false, message, errors });
    }

    /// <summary>
    /// Check if request is AJAX
    /// </summary>
    protected bool IsAjaxRequest()
    {
        return Request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }

    /// <summary>
    /// Get pagination parameters from query string
    /// </summary>
    protected (int pageNumber, int pageSize) GetPaginationParameters()
    {
        var pageNumber = Request.Query.ContainsKey("page") && int.TryParse(Request.Query["page"], out var p) ? p : 1;
        var pageSize = Request.Query.ContainsKey("size") && int.TryParse(Request.Query["size"], out var s) ? s : 10;
        
        // Ensure reasonable limits
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Max(5, Math.Min(100, pageSize));
        
        return (pageNumber, pageSize);
    }

    /// <summary>
    /// Set breadcrumb items for navigation
    /// </summary>
    protected void SetBreadcrumb(params (string text, string? url)[] items)
    {
        ViewBag.BreadcrumbItems = items;
    }

    /// <summary>
    /// Set page title
    /// </summary>
    protected void SetPageTitle(string title, string? subtitle = null)
    {
        ViewData["Title"] = title;
        if (!string.IsNullOrEmpty(subtitle))
        {
            ViewBag.PageSubtitle = subtitle;
        }
    }

    /// <summary>
    /// Override OnActionExecuting to add common data to ViewBag
    /// </summary>
    public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        // Add user context to ViewBag
        ViewBag.CurrentUserId = GetCurrentUserId();
        ViewBag.CurrentUserEmail = GetCurrentUserEmail();
        ViewBag.IsAdmin = IsAdmin;
        ViewBag.IsManagerOrAdmin = IsManagerOrAdmin;

        // Add controller and action names
        ViewBag.ControllerName = ControllerContext.ActionDescriptor.ControllerName;
        ViewBag.ActionName = ControllerContext.ActionDescriptor.ActionName;
    }

    /// <summary>
    /// Handle model state errors and set appropriate message
    /// </summary>
    protected IActionResult HandleModelStateErrors()
    {
        var errors = ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors)
            .Select(x => x.ErrorMessage)
            .ToList();

        var errorMessage = string.Join(", ", errors);
        SetErrorMessage($"Please correct the following errors: {errorMessage}");

        if (IsAjaxRequest())
        {
            return JsonError("Validation errors occurred", errors);
        }

        return View();
    }

    /// <summary>
    /// Log user action for audit purposes
    /// </summary>
    protected void LogUserAction(string action, string? details = null)
    {
        _logger.LogInformation("User {UserId} performed action: {Action}. Details: {Details}", 
            GetCurrentUserId(), action, details ?? "N/A");
    }
}
