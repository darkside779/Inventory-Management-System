using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Application.Features.Suppliers.Queries.GetAllSuppliers;
using InventoryManagement.Application.Features.Suppliers.Queries.GetSupplierById;
using InventoryManagement.Application.Features.Suppliers.Queries.GetSupplierWithProducts;
using InventoryManagement.Application.Features.Suppliers.Commands.CreateSupplier;
using InventoryManagement.Application.Features.Suppliers.Commands.UpdateSupplier;
using InventoryManagement.Application.Features.Suppliers.Commands.DeleteSupplier;
using InventoryManagement.WebUI.ViewModels.Suppliers;

namespace InventoryManagement.WebUI.Controllers;

/// <summary>
/// Controller for managing suppliers
/// </summary>
[Authorize]
public class SupplierController : BaseController
{
    public SupplierController(IMediator mediator, ILogger<SupplierController> logger)
        : base(mediator, logger)
    {
    }

    /// <summary>
    /// Display paginated list of suppliers
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] SupplierIndexViewModel model)
    {
        try
        {
            var query = new GetAllSuppliersQuery
            {
                PageNumber = model.CurrentPage,
                PageSize = model.PageSize,
                SearchTerm = model.SearchTerm,
                ActiveOnly = model.ActiveOnly,
                WithProductsOnly = model.WithProductsOnly,
                SortBy = model.SortBy,
                SortDirection = model.SortDirection
            };

            var response = await _mediator.Send(query);

            model.Suppliers = response.Suppliers;
            model.TotalCount = response.TotalCount;
            model.TotalPages = response.TotalPages;
            model.HasNextPage = response.HasNextPage;
            model.HasPreviousPage = response.HasPreviousPage;

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving suppliers for User {UserId}", GetCurrentUserIdAsInt());
            SetErrorMessage("An error occurred while loading suppliers. Please try again.");
            return View(new SupplierIndexViewModel());
        }
    }

    /// <summary>
    /// Display supplier details
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var query = new GetSupplierWithProductsQuery(id, activeProductsOnly: false, lowStockProductsOnly: false);
            var response = await _mediator.Send(query);

            if (!response.IsFound || response.Supplier == null)
            {
                SetErrorMessage("Supplier not found.");
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new SupplierDetailsViewModel
            {
                Id = response.Supplier.Id,
                Name = response.Supplier.Name,
                ContactPerson = response.Supplier.ContactPerson,
                Phone = response.Supplier.Phone,
                Email = response.Supplier.Email,
                Address = response.Supplier.Address,
                IsActive = response.Supplier.IsActive,
                CreatedAt = response.Supplier.CreatedAt,
                UpdatedAt = response.Supplier.UpdatedAt,
                Products = response.Products,
                TotalProducts = response.TotalProducts,
                ActiveProducts = response.ActiveProducts,
                InactiveProducts = response.InactiveProducts,
                LowStockProducts = response.LowStockProducts,
                TotalInventoryValue = response.TotalInventoryValue,
                AverageProductPrice = response.AverageProductPrice,
                HighestPrice = response.HighestPrice,
                LowestPrice = response.LowestPrice
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving supplier details {SupplierId} for User {UserId}", id, GetCurrentUserIdAsInt());
            SetErrorMessage("An error occurred while loading supplier details. Please try again.");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Display create supplier form
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Create()
    {
        return View(new CreateSupplierViewModel());
    }

    /// <summary>
    /// Handle supplier creation
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create(CreateSupplierViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var command = new CreateSupplierCommand
            {
                Name = model.Name,
                ContactInfo = model.ContactPerson,
                Phone = model.Phone,
                Email = model.Email,
                Address = model.Address,
                Website = model.Website,
                TaxNumber = model.TaxNumber,
                PaymentTerms = model.PaymentTerms,
                IsActive = model.IsActive,
                UserId = GetCurrentUserIdAsInt()
            };

            var response = await _mediator.Send(command);

            if (response.IsSuccess)
            {
                SetSuccessMessage("Supplier created successfully.");
                return RedirectToAction(nameof(Details), new { id = response.SupplierId });
            }

            // Add validation errors to ModelState
            foreach (var error in response.ValidationErrors)
            {
                foreach (var errorMessage in error.Value)
                {
                    ModelState.AddModelError(error.Key, errorMessage);
                }
            }

            if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                SetErrorMessage(response.ErrorMessage);
            }

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating supplier for User {UserId}", GetCurrentUserIdAsInt());
            SetErrorMessage("An error occurred while creating the supplier. Please try again.");
            return View(model);
        }
    }

    /// <summary>
    /// Display edit supplier form
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var query = new GetSupplierByIdQuery(id);
            var response = await _mediator.Send(query);

            if (!response.Success || response.Supplier == null)
            {
                SetErrorMessage("Supplier not found.");
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new EditSupplierViewModel
            {
                Id = response.Supplier.Id,
                Name = response.Supplier.Name,
                ContactPerson = response.Supplier.ContactPerson,
                Phone = response.Supplier.Phone,
                Email = response.Supplier.Email,
                Address = response.Supplier.Address,
                IsActive = response.Supplier.IsActive
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading supplier for edit {SupplierId} for User {UserId}", id, GetCurrentUserIdAsInt());
            SetErrorMessage("An error occurred while loading supplier. Please try again.");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Handle supplier updates
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(EditSupplierViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var command = new UpdateSupplierCommand
            {
                Id = model.Id,
                Name = model.Name,
                ContactInfo = model.ContactPerson,
                Phone = model.Phone,
                Email = model.Email,
                Address = model.Address,
                Website = model.Website,
                TaxNumber = model.TaxNumber,
                PaymentTerms = model.PaymentTerms,
                IsActive = model.IsActive,
                UserId = GetCurrentUserIdAsInt()
            };

            var response = await _mediator.Send(command);

            if (response.IsSuccess)
            {
                SetSuccessMessage("Supplier updated successfully.");
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }

            if (!response.IsFound)
            {
                SetErrorMessage("Supplier not found.");
                return RedirectToAction(nameof(Index));
            }

            // Add validation errors to ModelState
            foreach (var error in response.ValidationErrors)
            {
                foreach (var errorMessage in error.Value)
                {
                    ModelState.AddModelError(error.Key, errorMessage);
                }
            }

            if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                SetErrorMessage(response.ErrorMessage);
            }

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating supplier {SupplierId} for User {UserId}", model.Id, GetCurrentUserIdAsInt());
            SetErrorMessage("An error occurred while updating the supplier. Please try again.");
            return View(model);
        }
    }

    /// <summary>
    /// Display delete supplier confirmation
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var query = new GetSupplierWithProductsQuery(id, activeProductsOnly: false, lowStockProductsOnly: false);
            var response = await _mediator.Send(query);

            if (!response.IsFound || response.Supplier == null)
            {
                SetErrorMessage("Supplier not found.");
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new DeleteSupplierViewModel
            {
                Id = response.Supplier.Id,
                Name = response.Supplier.Name,
                ContactPerson = response.Supplier.ContactPerson,
                Phone = response.Supplier.Phone,
                Email = response.Supplier.Email,
                Address = response.Supplier.Address,
                IsActive = response.Supplier.IsActive,
                CreatedAt = response.Supplier.CreatedAt,
                UpdatedAt = response.Supplier.UpdatedAt,
                TotalProducts = response.TotalProducts,
                ActiveProducts = response.ActiveProducts,
                TotalInventoryValue = response.TotalInventoryValue,
                HasActiveProducts = response.ActiveProducts > 0,
                HasInventoryWithStock = response.Products.Any(p => p.TotalQuantity > 0),
                HasRecentTransactions = response.Products.Any(p => p.CreatedAt >= DateTime.UtcNow.AddDays(-30))
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading supplier for delete {SupplierId} for User {UserId}", id, GetCurrentUserIdAsInt());
            SetErrorMessage("An error occurred while loading supplier. Please try again.");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Handle supplier deletion
    /// </summary>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var command = new DeleteSupplierCommand(id, GetCurrentUserIdAsInt());
            var response = await _mediator.Send(command);

            if (response.IsSuccess)
            {
                SetSuccessMessage("Supplier deleted successfully.");
                return RedirectToAction(nameof(Index));
            }

            if (!response.IsFound)
            {
                SetErrorMessage("Supplier not found.");
                return RedirectToAction(nameof(Index));
            }

            SetErrorMessage(response.ErrorMessage ?? "Unable to delete supplier.");
            return RedirectToAction(nameof(Delete), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting supplier {SupplierId} for User {UserId}", id, GetCurrentUserIdAsInt());
            SetErrorMessage("An error occurred while deleting the supplier. Please try again.");
            return RedirectToAction(nameof(Delete), new { id });
        }
    }

    /// <summary>
    /// Toggle supplier status (AJAX)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> ToggleStatus([FromBody] ToggleStatusRequest request)
    {
        try
        {
            // Get current supplier
            var query = new GetSupplierByIdQuery(request.Id, includeProducts: true);
            var supplierResponse = await _mediator.Send(query);

            if (!supplierResponse.Success || supplierResponse.Supplier == null)
            {
                return Json(new { success = false, message = "Supplier not found." });
            }

            var command = new UpdateSupplierCommand
            {
                Id = request.Id,
                Name = supplierResponse.Supplier.Name,
                ContactInfo = supplierResponse.Supplier.ContactPerson,
                Phone = supplierResponse.Supplier.Phone,
                Email = supplierResponse.Supplier.Email,
                Address = supplierResponse.Supplier.Address,
                IsActive = !supplierResponse.Supplier.IsActive,
                UserId = GetCurrentUserIdAsInt()
            };

            var response = await _mediator.Send(command);

            if (response.IsSuccess)
            {
                var newStatus = command.IsActive ? "activated" : "deactivated";
                return Json(new { success = true, message = $"Supplier {newStatus} successfully." });
            }

            return Json(new { success = false, message = response.ErrorMessage ?? "Unable to toggle supplier status." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling supplier status {SupplierId} for User {UserId}", request.Id, GetCurrentUserIdAsInt());
            return Json(new { success = false, message = "An error occurred while updating supplier status." });
        }
    }

    /// <summary>
    /// Get supplier products (AJAX)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetSupplierProducts(int supplierId, bool activeOnly = false, bool lowStockOnly = false)
    {
        try
        {
            var query = new GetSupplierWithProductsQuery(supplierId, activeOnly, lowStockOnly);
            var response = await _mediator.Send(query);

            if (!response.IsFound)
            {
                return Json(new { success = false, message = "Supplier not found." });
            }

            var result = new
            {
                success = true,
                products = response.Products.Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    sku = p.SKU,
                    price = p.Price,
                    isActive = p.IsActive,
                    totalQuantity = p.TotalQuantity
                })
            };

            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving supplier products {SupplierId} for User {UserId}", supplierId, GetCurrentUserIdAsInt());
            return Json(new { success = false, message = "An error occurred while loading products." });
        }
    }

    public class ToggleStatusRequest
    {
        public int Id { get; set; }
    }
}
