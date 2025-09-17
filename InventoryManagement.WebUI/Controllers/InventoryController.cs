using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using MediatR;
using InventoryManagement.Application.Features.Inventory.Queries.GetAllInventory;
using InventoryManagement.Application.Features.Inventory.Queries.GetInventoryById;
using InventoryManagement.Application.Features.Inventory.Queries.GetInventoryByProduct;
using InventoryManagement.Application.Features.Inventory.Commands.AdjustStock;
using InventoryManagement.Application.Features.Inventory.Commands.ReserveStock;
using InventoryManagement.Application.Features.Inventory.Commands.TransferStock;
using InventoryManagement.WebUI.ViewModels.Inventory;
using InventoryManagement.WebUI.Controllers;

namespace InventoryManagement.WebUI.Controllers;

/// <summary>
/// Controller for inventory management operations
/// </summary>
[Authorize]
public class InventoryController : BaseController
{
    private readonly IMapper _mapper;

    public InventoryController(IMediator mediator, IMapper mapper, ILogger<InventoryController> logger)
        : base(mediator, logger)
    {
        _mapper = mapper;
    }

    /// <summary>
    /// Display paginated list of inventory records
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, 
        int? warehouseId = null, bool? lowStockOnly = null, string sortBy = "ProductName", string sortDirection = "asc")
    {
        try
        {
            var query = new GetAllInventoryQuery
            {
                PageNumber = page,
                PageSize = pageSize,
                SearchTerm = search,
                WarehouseId = warehouseId,
                LowStockOnly = lowStockOnly,
                SortBy = sortBy,
                SortDirection = sortDirection,
                ActiveOnly = true
            };

            var result = await _mediator.Send(query);

            var viewModel = new InventoryIndexViewModel
            {
                Inventories = result.Inventories.ToList(),
                CurrentPage = result.PageNumber,
                TotalPages = result.TotalPages,
                TotalCount = result.TotalCount,
                PageSize = result.PageSize,
                SearchTerm = search,
                WarehouseId = warehouseId,
                LowStockOnly = lowStockOnly,
                SortBy = sortBy,
                SortDirection = sortDirection,
                HasPreviousPage = result.HasPreviousPage,
                HasNextPage = result.HasNextPage,
                PageTitle = "Inventory Management"
            };

            SetBreadcrumb(
                ("Home", Url.Action("Index", "Home")),
                ("Inventory", null)
            );

            LogUserAction($"Viewed inventory list - Page {page}");
            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "retrieving inventory list");
        }
    }

    /// <summary>
    /// Display detailed inventory information
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var query = new GetInventoryByIdQuery(id);
            var inventory = await _mediator.Send(query);

            if (inventory == null)
            {
                SetErrorMessage("Inventory record not found.");
                return RedirectToAction(nameof(Index));
            }

            var viewModel = _mapper.Map<InventoryDetailsViewModel>(inventory);
            
            SetBreadcrumb(
                ("Home", Url.Action("Index", "Home")),
                ("Inventory", Url.Action("Index")),
                ("Details", null)
            );

            LogUserAction($"Viewed inventory details - ID: {id}");
            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "retrieving inventory details");
        }
    }

    /// <summary>
    /// Display stock adjustment form
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Adjust(int? inventoryId, int? productId, int? warehouseId)
    {
        try
        {
            var viewModel = new StockAdjustmentViewModel();

            if (inventoryId.HasValue)
            {
                var query = new GetInventoryByIdQuery(inventoryId.Value);
                var inventory = await _mediator.Send(query);
                
                if (inventory != null)
                {
                    viewModel.ProductId = inventory.ProductId;
                    viewModel.WarehouseId = inventory.WarehouseId;
                    viewModel.ProductName = inventory.ProductName;
                    viewModel.WarehouseName = inventory.WarehouseName;
                    viewModel.CurrentQuantity = inventory.Quantity;
                }
            }
            else if (productId.HasValue && warehouseId.HasValue)
            {
                viewModel.ProductId = productId.Value;
                viewModel.WarehouseId = warehouseId.Value;
            }

            SetBreadcrumb(
                ("Home", Url.Action("Index", "Home")),
                ("Inventory", Url.Action("Index")),
                ("Adjust Stock", null)
            );

            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading stock adjustment form");
        }
    }

    /// <summary>
    /// Process stock adjustment
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Adjust(StockAdjustmentViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return HandleModelStateErrors();
            }

            var command = new AdjustStockCommand
            {
                ProductId = model.ProductId,
                WarehouseId = model.WarehouseId,
                AdjustmentQuantity = model.AdjustmentQuantity,
                Reason = model.Reason,
                ReferenceNumber = model.ReferenceNumber,
                UserId = GetCurrentUserIdAsInt()
            };

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                SetSuccessMessage($"Stock adjusted successfully. New quantity: {result.NewQuantity}");
                LogUserAction($"Adjusted stock - Product: {model.ProductId}, Warehouse: {model.WarehouseId}, Adjustment: {model.AdjustmentQuantity}");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                SetErrorMessage(result.ErrorMessage ?? "An error occurred during the operation.");
                return View(model);
            }
        }
        catch (Exception ex)
        {
            return HandleException(ex, "adjusting stock");
        }
    }

    /// <summary>
    /// Display stock transfer form
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Transfer(int? inventoryId, int? productId)
    {
        try
        {
            var viewModel = new StockTransferViewModel();

            if (inventoryId.HasValue)
            {
                var query = new GetInventoryByIdQuery(inventoryId.Value);
                var inventory = await _mediator.Send(query);
                
                if (inventory != null)
                {
                    viewModel.ProductId = inventory.ProductId;
                    viewModel.SourceWarehouseId = inventory.WarehouseId;
                    viewModel.ProductName = inventory.ProductName;
                    viewModel.SourceWarehouseName = inventory.WarehouseName;
                    viewModel.AvailableQuantity = inventory.AvailableQuantity;
                }
            }
            else if (productId.HasValue)
            {
                viewModel.ProductId = productId.Value;
            }

            SetBreadcrumb(
                ("Home", Url.Action("Index", "Home")),
                ("Inventory", Url.Action("Index")),
                ("Transfer Stock", null)
            );

            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading stock transfer form");
        }
    }

    /// <summary>
    /// Process stock transfer
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Transfer(StockTransferViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return HandleModelStateErrors();
            }

            var command = new TransferStockCommand
            {
                ProductId = model.ProductId,
                SourceWarehouseId = model.SourceWarehouseId,
                DestinationWarehouseId = model.DestinationWarehouseId,
                Quantity = model.Quantity,
                Reason = model.Reason,
                ReferenceNumber = model.ReferenceNumber,
                UserId = GetCurrentUserIdAsInt()
            };

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                SetSuccessMessage($"Stock transferred successfully. Source: {result.SourceQuantity}, Destination: {result.DestinationQuantity}");
                LogUserAction($"Transferred stock - Product: {model.ProductId}, From: {model.SourceWarehouseId}, To: {model.DestinationWarehouseId}, Qty: {model.Quantity}");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                SetErrorMessage(result.ErrorMessage ?? "An error occurred during the operation.");
                return View(model);
            }
        }
        catch (Exception ex)
        {
            return HandleException(ex, "transferring stock");
        }
    }

    /// <summary>
    /// Display stock reservation form
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Reserve(int? inventoryId, int? productId, int? warehouseId)
    {
        try
        {
            var viewModel = new StockReservationViewModel();

            if (inventoryId.HasValue)
            {
                var query = new GetInventoryByIdQuery(inventoryId.Value);
                var inventory = await _mediator.Send(query);
                
                if (inventory != null)
                {
                    viewModel.ProductId = inventory.ProductId;
                    viewModel.WarehouseId = inventory.WarehouseId;
                    viewModel.ProductName = inventory.ProductName;
                    viewModel.WarehouseName = inventory.WarehouseName;
                    viewModel.AvailableQuantity = inventory.AvailableQuantity;
                }
            }
            else if (productId.HasValue && warehouseId.HasValue)
            {
                viewModel.ProductId = productId.Value;
                viewModel.WarehouseId = warehouseId.Value;
            }

            SetBreadcrumb(
                ("Home", Url.Action("Index", "Home")),
                ("Inventory", Url.Action("Index")),
                ("Reserve Stock", null)
            );

            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading stock reservation form");
        }
    }

    /// <summary>
    /// Process stock reservation
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Reserve(StockReservationViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return HandleModelStateErrors();
            }

            var command = new ReserveStockCommand
            {
                ProductId = model.ProductId,
                WarehouseId = model.WarehouseId,
                Quantity = model.Quantity,
                Reason = model.Reason,
                ReferenceNumber = model.ReferenceNumber,
                UserId = GetCurrentUserIdAsInt()
            };

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                SetSuccessMessage($"Stock reserved successfully. Available quantity: {result.AvailableQuantity}");
                LogUserAction($"Reserved stock - Product: {model.ProductId}, Warehouse: {model.WarehouseId}, Qty: {model.Quantity}");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                SetErrorMessage(result.ErrorMessage ?? "An error occurred during the operation.");
                return View(model);
            }
        }
        catch (Exception ex)
        {
            return HandleException(ex, "reserving stock");
        }
    }

    /// <summary>
    /// API endpoint to get inventory by product
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetByProduct(int productId)
    {
        try
        {
            var query = new GetInventoryByProductQuery(productId);
            var inventories = await _mediator.Send(query);

            return Json(new { success = true, data = inventories });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inventory for product {ProductId}", productId);
            return Json(new { success = false, message = "Error retrieving inventory data" });
        }
    }

    /// <summary>
    /// Display stock in form
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Administrator,Manager")]
    public IActionResult StockIn()
    {
        try
        {
            var viewModel = new StockAdjustmentViewModel();

            SetBreadcrumb(
                ("Home", Url.Action("Index", "Home")),
                ("Inventory", Url.Action("Index")),
                ("Stock In", null)
            );

            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading stock in form");
        }
    }

    /// <summary>
    /// Process stock in
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> StockIn(StockAdjustmentViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Ensure positive adjustment for stock in
            var adjustmentQuantity = Math.Abs(model.AdjustmentQuantity);

            var command = new AdjustStockCommand
            {
                ProductId = model.ProductId,
                WarehouseId = model.WarehouseId,
                AdjustmentQuantity = adjustmentQuantity,
                Reason = model.Reason ?? "Stock In",
                ReferenceNumber = model.ReferenceNumber,
                UserId = GetCurrentUserIdAsInt()
            };

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                SetSuccessMessage($"Stock added successfully. New quantity: {result.NewQuantity}");
                LogUserAction($"Stock In - Product: {model.ProductId}, Warehouse: {model.WarehouseId}, Quantity: {adjustmentQuantity}");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                SetErrorMessage(result.ErrorMessage ?? "An error occurred during the operation.");
                return View(model);
            }
        }
        catch (Exception ex)
        {
            return HandleException(ex, "processing stock in");
        }
    }

    /// <summary>
    /// Display stock out form
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Administrator,Manager")]
    public IActionResult StockOut()
    {
        try
        {
            var viewModel = new StockAdjustmentViewModel();

            SetBreadcrumb(
                ("Home", Url.Action("Index", "Home")),
                ("Inventory", Url.Action("Index")),
                ("Stock Out", null)
            );

            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading stock out form");
        }
    }

    /// <summary>
    /// Process stock out
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> StockOut(StockAdjustmentViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Ensure negative adjustment for stock out
            var adjustmentQuantity = -Math.Abs(model.AdjustmentQuantity);

            var command = new AdjustStockCommand
            {
                ProductId = model.ProductId,
                WarehouseId = model.WarehouseId,
                AdjustmentQuantity = adjustmentQuantity,
                Reason = model.Reason ?? "Stock Out",
                ReferenceNumber = model.ReferenceNumber,
                UserId = GetCurrentUserIdAsInt()
            };

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                SetSuccessMessage($"Stock removed successfully. New quantity: {result.NewQuantity}");
                LogUserAction($"Stock Out - Product: {model.ProductId}, Warehouse: {model.WarehouseId}, Quantity: {Math.Abs(adjustmentQuantity)}");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                SetErrorMessage(result.ErrorMessage ?? "An error occurred during the operation.");
                return View(model);
            }
        }
        catch (Exception ex)
        {
            return HandleException(ex, "processing stock out");
        }
    }

    /// <summary>
    /// API endpoint to get low stock alerts
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> LowStockAlerts()
    {
        try
        {
            var query = new GetAllInventoryQuery
            {
                LowStockOnly = true,
                PageSize = 100,
                ActiveOnly = true
            };

            var result = await _mediator.Send(query);
            var alerts = result.Inventories.Select(i => new
            {
                i.Id,
                i.ProductName,
                i.ProductSKU,
                i.WarehouseName,
                i.Quantity,
                i.AvailableQuantity,
                MinimumLevel = i.MinimumStockLevel,
                Shortage = i.MinimumStockLevel - i.Quantity
            });

            return Json(new { success = true, data = alerts });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving low stock alerts");
            return Json(new { success = false, message = "Error retrieving low stock alerts" });
        }
    }
}
