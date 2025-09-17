using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InventoryManagement.WebUI.Filters;

/// <summary>
/// Action filter to log user actions for audit purposes
/// </summary>
public class AuditActionFilter : ActionFilterAttribute
{
    private readonly ILogger<AuditActionFilter> _logger;

    public AuditActionFilter(ILogger<AuditActionFilter> logger)
    {
        _logger = logger;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userId = context.HttpContext.User.Identity?.Name ?? "Anonymous";
        var controller = context.ActionDescriptor.RouteValues["controller"];
        var action = context.ActionDescriptor.RouteValues["action"];
        var method = context.HttpContext.Request.Method;

        _logger.LogInformation("User {UserId} executing {Method} {Controller}/{Action}", 
            userId, method, controller, action);

        base.OnActionExecuting(context);
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        var userId = context.HttpContext.User.Identity?.Name ?? "Anonymous";
        var controller = context.ActionDescriptor.RouteValues["controller"];
        var action = context.ActionDescriptor.RouteValues["action"];

        if (context.Exception != null)
        {
            _logger.LogError(context.Exception, "User {UserId} encountered error in {Controller}/{Action}", 
                userId, controller, action);
        }
        else
        {
            _logger.LogInformation("User {UserId} completed {Controller}/{Action} successfully", 
                userId, controller, action);
        }

        base.OnActionExecuted(context);
    }
}

/// <summary>
/// Exception filter to handle unhandled exceptions gracefully
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var userId = context.HttpContext.User.Identity?.Name ?? "Anonymous";
        var controller = context.ActionDescriptor.RouteValues["controller"];
        var action = context.ActionDescriptor.RouteValues["action"];

        _logger.LogError(context.Exception, 
            "Unhandled exception occurred for user {UserId} in {Controller}/{Action}", 
            userId, controller, action);

        // Check if it's an AJAX request
        if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            context.Result = new JsonResult(new 
            { 
                success = false, 
                message = "An unexpected error occurred. Please try again." 
            });
        }
        else
        {
            // Set TempData message for regular requests
            context.HttpContext.Session.SetString("ErrorMessage", "An unexpected error occurred. Please try again.");
        }

        context.ExceptionHandled = true;
    }
}

/// <summary>
/// Action filter to validate model state and return appropriate responses
/// </summary>
public class ValidateModelStateFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            // Check if it's an AJAX request
            if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                context.Result = new JsonResult(new 
                { 
                    success = false, 
                    message = "Validation errors occurred", 
                    errors 
                });
            }
            else
            {
                // For regular requests, let the action handle model state errors
                var controller = context.Controller as Controller;
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                var errorMessage = string.Join(", ", errors);
                controller?.TempData.Add("ErrorMessage", $"Please correct the following errors: {errorMessage}");
            }
        }

        base.OnActionExecuting(context);
    }
}

/// <summary>
/// Action filter to add security headers
/// </summary>
public class SecurityHeadersFilter : ActionFilterAttribute
{
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        var response = context.HttpContext.Response;

        // Add security headers
        if (!response.Headers.ContainsKey("X-Content-Type-Options"))
            response.Headers["X-Content-Type-Options"] = "nosniff";

        if (!response.Headers.ContainsKey("X-Frame-Options"))
            response.Headers["X-Frame-Options"] = "DENY";

        if (!response.Headers.ContainsKey("X-XSS-Protection"))
            response.Headers["X-XSS-Protection"] = "1; mode=block";

        if (!response.Headers.ContainsKey("Referrer-Policy"))
            response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        base.OnActionExecuted(context);
    }
}
