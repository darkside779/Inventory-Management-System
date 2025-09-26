using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using InventoryManagement.Application.Features.Warehouses.Queries.GetAllWarehouses;
using InventoryManagement.Application.Features.Warehouses.Queries.GetWarehouseById;
using InventoryManagement.Application.Features.Warehouses.Queries.GetWarehouseWithInventory;
using InventoryManagement.Application.Features.Warehouses.Commands.CreateWarehouse;
using InventoryManagement.Application.Features.Warehouses.Commands.UpdateWarehouse;
using InventoryManagement.Application.Features.Warehouses.Commands.DeleteWarehouse;
using InventoryManagement.WebUI.ViewModels.Warehouses;

namespace InventoryManagement.WebUI.Controllers;

/// <summary>
/// Controller for warehouse management operations
/// </summary>
[Authorize]
public class WarehouseController : BaseController
{
    public WarehouseController(IMediator mediator, ILogger<WarehouseController> logger)
        : base(mediator, logger)
    {
    }

    /// <summary>
    /// Display paginated list of warehouses
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        bool activeOnly = true,
        string sortBy = "Name",
        string sortDirection = "asc",
        int? minCapacity = null,
        int? maxCapacity = null)
    {
        try
        {
            var query = new GetAllWarehousesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                ActiveOnly = activeOnly,
                SortBy = sortBy,
                SortDirection = sortDirection,
                MinCapacity = minCapacity,
                MaxCapacity = maxCapacity
            };

            var result = await _mediator.Send(query);

            var viewModel = new WarehouseIndexViewModel
            {
                Warehouses = result.Warehouses,
                CurrentPage = result.PageNumber,
                TotalPages = result.TotalPages,
                TotalCount = result.TotalCount,
                PageSize = result.PageSize,
                SearchTerm = searchTerm,
                ActiveOnly = activeOnly,
                SortBy = sortBy,
                SortDirection = sortDirection,
                MinCapacity = minCapacity,
                MaxCapacity = maxCapacity,
                HasPreviousPage = result.HasPreviousPage,
                HasNextPage = result.HasNextPage
            };

            LogUserAction($"Viewed warehouses list - Page: {pageNumber}, Search: {searchTerm ?? "None"}");
            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading warehouses");
        }
    }

    /// <summary>
    /// Display warehouse details with inventory information
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var query = new GetWarehouseWithInventoryQuery
            {
                Id = id,
                ActiveOnly = true,
                LowStockOnly = false
            };

            var result = await _mediator.Send(query);

            if (result == null)
            {
                SetErrorMessage("Warehouse not found.");
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new WarehouseDetailsViewModel
            {
                Id = result.Warehouse.Id,
                Name = result.Warehouse.Name,
                Location = result.Warehouse.Location,
                Address = result.Warehouse.Description,
                Capacity = result.Warehouse.Capacity,
                IsActive = result.Warehouse.IsActive,
                CreatedAt = result.Warehouse.CreatedAt,
                UpdatedAt = result.Warehouse.UpdatedAt,
                InventoryItems = result.InventoryItems,
                TotalProducts = result.TotalProducts,
                TotalQuantity = result.TotalQuantity,
                TotalValue = result.TotalValue,
                LowStockItemsCount = result.LowStockItemsCount,
                UtilizationPercentage = result.UtilizationPercentage
            };

            LogUserAction($"Viewed warehouse details - ID: {id}, Name: {result.Warehouse.Name}");
            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading warehouse details");
        }
    }

    /// <summary>
    /// Display create warehouse form
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Administrator,Manager")]
    public IActionResult Create()
    {
        var viewModel = new CreateWarehouseViewModel();
        return View(viewModel);
    }

    /// <summary>
    /// Handle warehouse creation
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> Create(CreateWarehouseViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return HandleModelStateErrors();
            }

            var command = new CreateWarehouseCommand
            {
                Name = model.Name,
                Location = model.Location,
                Address = model.Address,
                ContactPhone = model.ContactPhone,
                ContactEmail = model.ContactEmail,
                Capacity = model.Capacity,
                IsActive = model.IsActive,
                UserId = GetCurrentUserIdAsInt()
            };

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                SetSuccessMessage($"Warehouse '{result.WarehouseName}' created successfully.");
                LogUserAction($"Created warehouse - Name: {model.Name}, Location: {model.Location}");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                SetErrorMessage(result.ErrorMessage ?? "An error occurred while creating the warehouse.");
                return View(model);
            }
        }
        catch (Exception ex)
        {
            return HandleException(ex, "creating warehouse");
        }
    }

    /// <summary>
    /// Display edit warehouse form
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var query = new GetWarehouseByIdQuery { Id = id };
            var warehouse = await _mediator.Send(query);

            if (warehouse == null)
            {
                SetErrorMessage("Warehouse not found.");
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new EditWarehouseViewModel
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Location = warehouse.Location,
                Address = warehouse.Description,
                Capacity = warehouse.Capacity,
                IsActive = warehouse.IsActive
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading warehouse for editing");
        }
    }

    /// <summary>
    /// Handle warehouse update
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> Edit(EditWarehouseViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return HandleModelStateErrors();
            }

            var command = new UpdateWarehouseCommand
            {
                Id = model.Id,
                Name = model.Name,
                Location = model.Location,
                Address = model.Address,
                ContactPhone = model.ContactPhone,
                ContactEmail = model.ContactEmail,
                Capacity = model.Capacity,
                IsActive = model.IsActive,
                UserId = GetCurrentUserIdAsInt()
            };

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                SetSuccessMessage($"Warehouse '{result.WarehouseName}' updated successfully.");
                LogUserAction($"Updated warehouse - ID: {model.Id}, Name: {model.Name}");
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            else
            {
                SetErrorMessage(result.ErrorMessage ?? "An error occurred while updating the warehouse.");
                return View(model);
            }
        }
        catch (Exception ex)
        {
            return HandleException(ex, "updating warehouse");
        }
    }

    /// <summary>
    /// Display delete warehouse confirmation
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var query = new GetWarehouseWithInventoryQuery
            {
                Id = id,
                ActiveOnly = false,
                LowStockOnly = false
            };

            var result = await _mediator.Send(query);

            if (result == null)
            {
                SetErrorMessage("Warehouse not found.");
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new DeleteWarehouseViewModel
            {
                Id = result.Warehouse.Id,
                Name = result.Warehouse.Name,
                Location = result.Warehouse.Location,
                Address = result.Warehouse.Description,
                Capacity = result.Warehouse.Capacity,
                IsActive = result.Warehouse.IsActive,
                CreatedAt = result.Warehouse.CreatedAt,
                UpdatedAt = result.Warehouse.UpdatedAt,
                TotalProducts = result.TotalProducts,
                TotalQuantity = result.TotalQuantity,
                TotalValue = result.TotalValue,
                HasActiveInventory = result.InventoryItems.Any(i => i.IsActive && i.Quantity > 0)
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading warehouse for deletion");
        }
    }

    /// <summary>
    /// Handle warehouse deletion
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var command = new DeleteWarehouseCommand
            {
                Id = id,
                UserId = GetCurrentUserIdAsInt()
            };

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                SetSuccessMessage($"Warehouse '{result.WarehouseName}' has been deactivated successfully.");
                LogUserAction($"Deleted warehouse - ID: {id}, Name: {result.WarehouseName}");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                SetErrorMessage(result.ErrorMessage ?? "An error occurred while deleting the warehouse.");
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
        catch (Exception ex)
        {
            return HandleException(ex, "deleting warehouse");
        }
    }

    /// <summary>
    /// Get warehouse inventory as JSON for AJAX calls
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetWarehouseInventory(int id, bool lowStockOnly = false)
    {
        try
        {
            var query = new GetWarehouseWithInventoryQuery
            {
                Id = id,
                ActiveOnly = true,
                LowStockOnly = lowStockOnly
            };

            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound("Warehouse not found");
            }

            return Json(new
            {
                success = true,
                warehouse = result.Warehouse,
                inventoryItems = result.InventoryItems,
                summary = new
                {
                    totalProducts = result.TotalProducts,
                    totalQuantity = result.TotalQuantity,
                    totalValue = result.TotalValue,
                    lowStockItemsCount = result.LowStockItemsCount,
                    utilizationPercentage = result.UtilizationPercentage
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading warehouse inventory for warehouse {WarehouseId}", id);
            return Json(new { success = false, message = "Error loading warehouse inventory" });
        }
    }

    /// <summary>
    /// Toggle warehouse active status via AJAX
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        try
        {
            // Get current warehouse
            var warehouseQuery = new GetWarehouseByIdQuery { Id = id };
            var warehouse = await _mediator.Send(warehouseQuery);

            if (warehouse == null)
            {
                return Json(new { success = false, message = "Warehouse not found" });
            }

            // Update status
            var command = new UpdateWarehouseCommand
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Location = warehouse.Location,
                Address = warehouse.Description,
                Capacity = warehouse.Capacity,
                IsActive = !warehouse.IsActive,
                UserId = GetCurrentUserIdAsInt()
            };

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                LogUserAction($"Toggled warehouse status - ID: {id}, Active: {command.IsActive}");
                return Json(new 
                { 
                    success = true, 
                    isActive = command.IsActive,
                    message = $"Warehouse {(command.IsActive ? "activated" : "deactivated")} successfully"
                });
            }
            else
            {
                return Json(new { success = false, message = result.ErrorMessage });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling warehouse status for warehouse {WarehouseId}", id);
            return Json(new { success = false, message = "Error updating warehouse status" });
        }
    }
}
