using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using MediatR;
using InventoryManagement.WebUI.ViewModels.Customer;
using InventoryManagement.Application.Features.Customers.Queries;
using InventoryManagement.Application.Features.Customers.Commands;

namespace InventoryManagement.WebUI.Controllers;

/// <summary>
/// Controller for customer management operations
/// </summary>
[Authorize(Roles = "Administrator,Manager")]
public class CustomerController : BaseController
{
    public CustomerController(IMediator mediator, ILogger<CustomerController> logger) 
        : base(mediator, logger)
    {
    }

    /// <summary>
    /// Display customer list with filtering and paging
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(CustomerFilterViewModel filter, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var query = new GetCustomersQuery
            {
                SearchTerm = filter.SearchTerm,
                CustomerType = filter.CustomerType,
                IsActive = filter.IsActive ?? true, // Default to active customers
                RegisteredFrom = filter.RegisteredFrom,
                RegisteredTo = filter.RegisteredTo,
                Page = pageNumber,
                PageSize = pageSize,
                SortBy = filter.SortBy ?? "FullName",
                SortDirection = filter.SortDirection ?? "asc"
            };

            var response = await _mediator.Send(query);

            if (response.IsSuccess && response.Data != null)
            {
                var viewModel = new CustomerIndexViewModel
                {
                    Customers = response.Data,
                    Filter = filter,
                    TotalCount = response.TotalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)response.TotalCount / pageSize)
                };

                return View(viewModel);
            }

            SetErrorMessage(response.ErrorMessage ?? "Failed to load customers");
            return View(new CustomerIndexViewModel());
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading customers");
        }
    }

    /// <summary>
    /// Display customer details
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var query = new GetCustomerByIdQuery { Id = id };
            var response = await _mediator.Send(query);

            if (response.IsSuccess && response.Customer != null)
            {
                var viewModel = new CustomerDetailsViewModel
                {
                    Id = response.Customer.Id,
                    CustomerCode = response.Customer.CustomerCode,
                    FullName = response.Customer.FullName,
                    CompanyName = response.Customer.CompanyName,
                    Email = response.Customer.Email,
                    PhoneNumber = response.Customer.PhoneNumber,
                    Address = response.Customer.Address,
                    CustomerType = response.Customer.CustomerType,
                    Balance = response.Customer.Balance,
                    CreditLimit = response.Customer.CreditLimit,
                    PaymentTermsDays = response.Customer.PaymentTermsDays,
                    TaxId = response.Customer.TaxId,
                    Notes = response.Customer.Notes,
                    IsActive = response.Customer.IsActive,
                    RegisteredDate = response.Customer.RegisteredDate,
                    LastPurchaseDate = response.Customer.LastPurchaseDate,
                    TotalPurchases = response.Customer.TotalPurchases
                };

                return View(viewModel);
            }

            SetErrorMessage("Customer not found");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading customer details");
        }
    }

    /// <summary>
    /// Display create customer form
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        var viewModel = new CreateCustomerViewModel();
        PopulateCustomerDropdowns(viewModel);
        return View(viewModel);
    }

    /// <summary>
    /// Handle customer creation
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCustomerViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                PopulateCustomerDropdowns(model);
                return View(model);
            }

            var command = new CreateCustomerCommand
            {
                FullName = model.FullName,
                CompanyName = model.CompanyName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                CustomerType = model.CustomerType,
                CreditLimit = model.CreditLimit,
                PaymentTermsDays = model.PaymentTermsDays,
                TaxId = model.TaxId,
                Notes = model.Notes,
                CreatedByUserId = GetCurrentUserIdAsInt()
            };

            var response = await _mediator.Send(command);

            if (response.IsSuccess)
            {
                SetSuccessMessage($"Customer '{model.FullName}' created successfully.");
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", response.ErrorMessage ?? "Failed to create customer");
            PopulateCustomerDropdowns(model);
            return View(model);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "creating customer");
        }
    }

    /// <summary>
    /// Display edit customer form
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var query = new GetCustomerByIdQuery { Id = id };
            var response = await _mediator.Send(query);

            if (response.IsSuccess && response.Customer != null)
            {
                var viewModel = new EditCustomerViewModel
                {
                    Id = response.Customer.Id,
                    CustomerCode = response.Customer.CustomerCode,
                    FullName = response.Customer.FullName,
                    CompanyName = response.Customer.CompanyName,
                    Email = response.Customer.Email,
                    PhoneNumber = response.Customer.PhoneNumber,
                    Address = response.Customer.Address,
                    CustomerType = response.Customer.CustomerType,
                    CreditLimit = response.Customer.CreditLimit,
                    PaymentTermsDays = response.Customer.PaymentTermsDays,
                    TaxId = response.Customer.TaxId,
                    Notes = response.Customer.Notes,
                    IsActive = response.Customer.IsActive
                };

                PopulateCustomerDropdowns(viewModel);
                return View(viewModel);
            }

            SetErrorMessage("Customer not found");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading customer for editing");
        }
    }

    /// <summary>
    /// Handle customer update
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditCustomerViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                PopulateCustomerDropdowns(model);
                return View(model);
            }

            var command = new UpdateCustomerCommand
            {
                Id = model.Id,
                FullName = model.FullName,
                CompanyName = model.CompanyName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                CustomerType = model.CustomerType,
                CreditLimit = model.CreditLimit,
                PaymentTermsDays = model.PaymentTermsDays,
                TaxId = model.TaxId,
                Notes = model.Notes,
                IsActive = model.IsActive,
                UpdatedByUserId = GetCurrentUserIdAsInt()
            };

            var response = await _mediator.Send(command);

            if (response.IsSuccess)
            {
                SetSuccessMessage($"Customer '{model.FullName}' updated successfully.");
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }

            ModelState.AddModelError("", response.ErrorMessage ?? "Failed to update customer");
            PopulateCustomerDropdowns(model);
            return View(model);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "updating customer");
        }
    }

    /// <summary>
    /// Display delete customer confirmation
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var query = new GetCustomerByIdQuery { Id = id };
            var response = await _mediator.Send(query);

            if (response.IsSuccess && response.Customer != null)
            {
                var viewModel = new CustomerDetailsViewModel
                {
                    Id = response.Customer.Id,
                    CustomerCode = response.Customer.CustomerCode,
                    FullName = response.Customer.FullName,
                    CompanyName = response.Customer.CompanyName,
                    Email = response.Customer.Email,
                    Balance = response.Customer.Balance,
                    TotalPurchases = response.Customer.TotalPurchases,
                    IsActive = response.Customer.IsActive
                };

                return View(viewModel);
            }

            SetErrorMessage("Customer not found");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading customer for deletion");
        }
    }

    /// <summary>
    /// Handle customer deletion
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var command = new DeleteCustomerCommand
            {
                Id = id,
                DeletedByUserId = GetCurrentUserIdAsInt()
            };

            var response = await _mediator.Send(command);

            if (response.IsSuccess)
            {
                SetSuccessMessage("Customer deleted successfully.");
                return RedirectToAction(nameof(Index));
            }

            SetErrorMessage(response.ErrorMessage ?? "Failed to delete customer");
            return RedirectToAction(nameof(Delete), new { id });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "deleting customer");
        }
    }

    /// <summary>
    /// Toggle customer active status via AJAX
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        try
        {
            var command = new ToggleCustomerStatusCommand
            {
                Id = id,
                UpdatedByUserId = GetCurrentUserIdAsInt()
            };

            var response = await _mediator.Send(command);

            if (response.IsSuccess)
            {
                return Json(new { success = true, message = "Customer status updated successfully" });
            }

            return Json(new { success = false, message = response.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling customer status for customer {CustomerId}", id);
            return Json(new { success = false, message = "Error toggling customer status" });
        }
    }

    /// <summary>
    /// Populate dropdown lists for customer forms
    /// </summary>
    private void PopulateCustomerDropdowns(dynamic viewModel)
    {
        viewModel.CustomerTypes = new SelectList(new[]
        {
            new { Value = "Individual", Text = "Individual" },
            new { Value = "Business", Text = "Business" },
            new { Value = "Corporate", Text = "Corporate" }
        }, "Value", "Text");

        viewModel.PaymentTermsOptions = new SelectList(new[]
        {
            new { Value = 0, Text = "Cash on Delivery" },
            new { Value = 7, Text = "7 Days" },
            new { Value = 15, Text = "15 Days" },
            new { Value = 30, Text = "30 Days" },
            new { Value = 60, Text = "60 Days" },
            new { Value = 90, Text = "90 Days" }
        }, "Value", "Text");
    }
}
