using InventoryManagement.Application.Features.Transactions.Commands.CreateAdjustmentTransaction;
using InventoryManagement.Application.Features.Transactions.Commands.CreateStockInTransaction;
using InventoryManagement.Application.Features.Transactions.Commands.CreateStockOutTransaction;
using InventoryManagement.Application.Features.Transactions.Queries.GetAllTransactions;
using InventoryManagement.Application.Features.Transactions.Queries.GetTransactionById;
using InventoryManagement.Application.Features.Transactions.Queries.GetTransactionsByProduct;
using InventoryManagement.Application.Features.Products.Queries.GetAllProducts;
using InventoryManagement.Application.Features.Warehouses.Queries.GetAllWarehouses;
using InventoryManagement.Domain.Enums;
using InventoryManagement.WebUI.ViewModels.Transactions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.Controllers;

/// <summary>
/// Controller for managing inventory transactions
/// </summary>
[Authorize]
public class TransactionController : BaseController
{
    public TransactionController(IMediator mediator, ILogger<TransactionController> logger) 
        : base(mediator, logger)
    {
    }

    /// <summary>
    /// Display list of transactions with filtering and pagination
    /// </summary>
    /// <param name="filter">Filter criteria</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Transaction index view</returns>
    [HttpGet]
    public async Task<IActionResult> Index(TransactionFilterViewModel filter, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            filter ??= new TransactionFilterViewModel();

            var query = new GetAllTransactionsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = filter.SearchTerm,
                TransactionType = filter.TransactionType,
                ProductId = filter.ProductId,
                WarehouseId = filter.WarehouseId,
                StartDate = filter.StartDate,
                EndDate = filter.EndDate,
                SortBy = filter.SortBy,
                SortDirection = filter.SortDirection
            };

            var response = await _mediator.Send(query);

            if (!response.Success)
            {
                SetErrorMessage(response.ErrorMessage ?? "Failed to load transactions");
                return View(new TransactionIndexViewModel());
            }

            // Load dropdown data
            var (products, warehouses) = await LoadDropdownDataAsync();

            var viewModel = new TransactionIndexViewModel
            {
                Transactions = response.Transactions,
                Filter = filter,
                Pagination = new PaginationViewModel
                {
                    PageNumber = response.PageNumber,
                    PageSize = response.PageSize,
                    TotalCount = response.TotalCount
                },
                Products = new SelectList(products, "Value", "Text", filter.ProductId),
                Warehouses = new SelectList(warehouses, "Value", "Text", filter.WarehouseId),
                TransactionTypes = new SelectList(GetTransactionTypeOptions(), "Value", "Text", filter.TransactionType?.ToString())
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while loading transaction index");
            SetErrorMessage("An error occurred while loading transactions");
            return View(new TransactionIndexViewModel());
        }
    }

    /// <summary>
    /// Display transaction details
    /// </summary>
    /// <param name="id">Transaction ID</param>
    /// <returns>Transaction details view</returns>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var query = new GetTransactionByIdQuery(id);
            var response = await _mediator.Send(query);

            if (!response.Success)
            {
                SetErrorMessage(response.ErrorMessage ?? "Failed to load transaction details");
                return RedirectToAction(nameof(Index));
            }

            if (!response.IsFound || response.Transaction == null)
            {
                SetErrorMessage("Transaction not found");
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new TransactionDetailsViewModel
            {
                Transaction = response.Transaction
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while loading transaction details for ID: {TransactionId}", id);
            SetErrorMessage("An error occurred while loading transaction details");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Display create stock in form
    /// </summary>
    /// <returns>Create stock in view</returns>
    [HttpGet]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> CreateStockIn()
    {
        try
        {
            var (products, warehouses) = await LoadDropdownDataAsync();
            
            var viewModel = new CreateStockInViewModel
            {
                Products = new SelectList(products, "Value", "Text"),
                Warehouses = new SelectList(warehouses, "Value", "Text")
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while loading create stock in form");
            SetErrorMessage("An error occurred while loading the form");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Process create stock in form submission
    /// </summary>
    /// <param name="model">Stock in model</param>
    /// <returns>Redirect to index or return view with errors</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> CreateStockIn(CreateStockInViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var (dropdownProducts, dropdownWarehouses) = await LoadDropdownDataAsync();
                model.Products = new SelectList(dropdownProducts, "Value", "Text", model.ProductId);
                model.Warehouses = new SelectList(dropdownWarehouses, "Value", "Text", model.WarehouseId);
                return View(model);
            }

            var command = new CreateStockInTransactionCommand
            {
                ProductId = model.ProductId,
                WarehouseId = model.WarehouseId,
                UserId = GetCurrentUserIdAsInt(),
                Quantity = model.Quantity,
                UnitCost = model.UnitCost,
                Reason = model.Reason,
                ReferenceNumber = model.ReferenceNumber,
                Notes = model.Notes
            };

            var response = await _mediator.Send(command);

            if (response.IsSuccess)
            {
                SetSuccessMessage($"Stock in transaction created successfully. Added {model.Quantity} units to inventory.");
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", response.ErrorMessage ?? "Failed to create stock in transaction");
            var (products, warehouses) = await LoadDropdownDataAsync();
            model.Products = new SelectList(products, "Value", "Text", model.ProductId);
            model.Warehouses = new SelectList(warehouses, "Value", "Text", model.WarehouseId);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating stock in transaction");
            ModelState.AddModelError("", "An error occurred while creating the transaction");
            var (products, warehouses) = await LoadDropdownDataAsync();
            model.Products = new SelectList(products, "Value", "Text", model.ProductId);
            model.Warehouses = new SelectList(warehouses, "Value", "Text", model.WarehouseId);
            return View(model);
        }
    }

    /// <summary>
    /// Display create stock out form
    /// </summary>
    /// <returns>Create stock out view</returns>
    [HttpGet]
    [Authorize(Roles = "Administrator,Manager,Staff")]
    public async Task<IActionResult> CreateStockOut()
    {
        try
        {
            var (products, warehouses) = await LoadDropdownDataAsync();

            var viewModel = new CreateStockOutViewModel
            {
                Products = new SelectList(products, "Value", "Text"),
                Warehouses = new SelectList(warehouses, "Value", "Text")
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while loading create stock out form");
            SetErrorMessage("An error occurred while loading the form");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Process create stock out form submission
    /// </summary>
    /// <param name="model">Stock out model</param>
    /// <returns>Redirect to index or return view with errors</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator,Manager,Staff")]
    public async Task<IActionResult> CreateStockOut(CreateStockOutViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var (dropdownProducts2, dropdownWarehouses2) = await LoadDropdownDataAsync();
                model.Products = new SelectList(dropdownProducts2, "Value", "Text", model.ProductId);
                model.Warehouses = new SelectList(dropdownWarehouses2, "Value", "Text", model.WarehouseId);
                return View(model);
            }

            var command = new CreateStockOutTransactionCommand
            {
                ProductId = model.ProductId,
                WarehouseId = model.WarehouseId,
                UserId = GetCurrentUserIdAsInt(),
                Quantity = model.Quantity,
                UnitCost = model.UnitCost,
                Reason = model.Reason,
                ReferenceNumber = model.ReferenceNumber,
                Notes = model.Notes
            };

            var response = await _mediator.Send(command);

            if (response.IsSuccess)
            {
                SetSuccessMessage($"Stock out transaction created successfully. Removed {model.Quantity} units from inventory.");
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", response.ErrorMessage ?? "Failed to create stock out transaction");
            var (products, warehouses) = await LoadDropdownDataAsync();
            model.Products = new SelectList(products, "Value", "Text", model.ProductId);
            model.Warehouses = new SelectList(warehouses, "Value", "Text", model.WarehouseId);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating stock out transaction");
            ModelState.AddModelError("", "An error occurred while creating the transaction");
            var (products, warehouses) = await LoadDropdownDataAsync();
            model.Products = new SelectList(products, "Value", "Text", model.ProductId);
            model.Warehouses = new SelectList(warehouses, "Value", "Text", model.WarehouseId);
            return View(model);
        }
    }

    /// <summary>
    /// Display create adjustment form
    /// </summary>
    /// <returns>Create adjustment view</returns>
    [HttpGet]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> CreateAdjustment()
    {
        try
        {
            var (products, warehouses) = await LoadDropdownDataAsync();

            var viewModel = new CreateAdjustmentViewModel
            {
                Products = new SelectList(products, "Value", "Text"),
                Warehouses = new SelectList(warehouses, "Value", "Text")
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while loading create adjustment form");
            SetErrorMessage("An error occurred while loading the form");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Process create adjustment form submission
    /// </summary>
    /// <param name="model">Adjustment model</param>
    /// <returns>Redirect to index or return view with errors</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> CreateAdjustment(CreateAdjustmentViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var (dropdownProducts3, dropdownWarehouses3) = await LoadDropdownDataAsync();
                model.Products = new SelectList(dropdownProducts3, "Value", "Text", model.ProductId);
                model.Warehouses = new SelectList(dropdownWarehouses3, "Value", "Text", model.WarehouseId);
                return View(model);
            }

            var command = new CreateAdjustmentTransactionCommand
            {
                ProductId = model.ProductId,
                WarehouseId = model.WarehouseId,
                UserId = GetCurrentUserIdAsInt(),
                QuantityAdjustment = model.QuantityAdjustment,
                Reason = model.Reason,
                ReferenceNumber = model.ReferenceNumber,
                Notes = model.Notes
            };

            var response = await _mediator.Send(command);

            if (response.IsSuccess)
            {
                var adjustmentType = model.QuantityAdjustment > 0 ? "increased" : "decreased";
                SetSuccessMessage($"Adjustment transaction created successfully. Stock {adjustmentType} by {Math.Abs(model.QuantityAdjustment)} units.");
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", response.ErrorMessage ?? "Failed to create adjustment transaction");
            var (products, warehouses) = await LoadDropdownDataAsync();
            model.Products = new SelectList(products, "Value", "Text", model.ProductId);
            model.Warehouses = new SelectList(warehouses, "Value", "Text", model.WarehouseId);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating adjustment transaction");
            ModelState.AddModelError("", "An error occurred while creating the transaction");
            var (products, warehouses) = await LoadDropdownDataAsync();
            model.Products = new SelectList(products, "Value", "Text", model.ProductId);
            model.Warehouses = new SelectList(warehouses, "Value", "Text", model.WarehouseId);
            return View(model);
        }
    }

    /// <summary>
    /// Display transaction history for a product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="warehouseId">Optional warehouse ID</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Transaction history view</returns>
    [HttpGet]
    public async Task<IActionResult> History(
        int productId,
        int? warehouseId = null,
        int pageNumber = 1,
        int pageSize = 20)
    {
        try
        {
            var query = new GetTransactionsByProductQuery(productId)
            {
                WarehouseId = warehouseId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var response = await _mediator.Send(query);

            if (!response.Success)
            {
                SetErrorMessage(response.ErrorMessage ?? "Failed to load transaction history");
                return RedirectToAction(nameof(Index));
            }

            if (!response.IsFound)
            {
                SetErrorMessage("Product not found");
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new TransactionHistoryViewModel
            {
                Product = response.Product,
                Transactions = response.Transactions,
                Statistics = new Application.DTOs.TransactionSummaryDto
                {
                    ProductId = productId,
                    ProductName = response.Product?.Name ?? "",
                    ProductSKU = response.Product?.SKU ?? "",
                    TotalStockIn = response.Statistics.TotalStockIn,
                    TotalStockOut = response.Statistics.TotalStockOut,
                    TotalAdjustments = response.Statistics.TotalAdjustments,
                    TotalStockInValue = response.Statistics.TotalStockInValue,
                    TotalStockOutValue = response.Statistics.TotalStockOutValue
                },
                Pagination = new PaginationViewModel
                {
                    PageNumber = response.PageNumber,
                    PageSize = response.PageSize,
                    TotalCount = response.TotalCount
                }
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while loading transaction history for product ID: {ProductId}", productId);
            SetErrorMessage("An error occurred while loading transaction history");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// API endpoint to get current stock level for AJAX calls
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="warehouseId">Warehouse ID</param>
    /// <returns>JSON response with current stock</returns>
    [HttpGet]
    public IActionResult GetCurrentStock(int productId, int warehouseId)
    {
        try
        {
            // This would typically use an inventory query, but for now we'll return a placeholder
            // In a real implementation, you'd query the inventory repository
            return Json(new { success = true, currentStock = 0 });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting current stock for Product: {ProductId}, Warehouse: {WarehouseId}", productId, warehouseId);
            return Json(new { success = false, error = "Failed to get current stock" });
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Load dropdown data for products and warehouses
    /// </summary>
    /// <returns>Tuple of products and warehouses select list items</returns>
    private async Task<(List<SelectListItem> products, List<SelectListItem> warehouses)> LoadDropdownDataAsync()
    {
        var products = new List<SelectListItem>
        {
            new() { Value = "", Text = "-- Select Product --" }
        };

        var warehouses = new List<SelectListItem>
        {
            new() { Value = "", Text = "-- Select Warehouse --" }
        };

        try
        {
            // Load products
            var productsQuery = new GetAllProductsQuery 
            { 
                PageNumber = 1, 
                PageSize = 1000, // Get all products for dropdown
                ActiveOnly = true 
            };
            var productsResponse = await _mediator.Send(productsQuery);
            
            products.AddRange(productsResponse.Items.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Name} - {p.SKU}"
            }));

            // Load warehouses
            var warehousesQuery = new GetAllWarehousesQuery 
            { 
                PageNumber = 1, 
                PageSize = 1000, // Get all warehouses for dropdown
                ActiveOnly = true 
            };
            var warehousesResponse = await _mediator.Send(warehousesQuery);
            
            warehouses.AddRange(warehousesResponse.Warehouses.Select(w => new SelectListItem
            {
                Value = w.Id.ToString(),
                Text = w.Name
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dropdown data");
        }

        return (products, warehouses);
    }

    /// <summary>
    /// Get transaction type options for dropdown
    /// </summary>
    /// <returns>List of select list items for transaction types</returns>
    private static List<SelectListItem> GetTransactionTypeOptions()
    {
        return new List<SelectListItem>
        {
            new() { Value = "", Text = "-- All Types --" },
            new() { Value = TransactionType.StockIn.ToString(), Text = "Stock In" },
            new() { Value = TransactionType.StockOut.ToString(), Text = "Stock Out" },
            new() { Value = TransactionType.Adjustment.ToString(), Text = "Adjustment" }
        };
    }

    #endregion
}
