using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Features.Reports.Queries.GetDashboardSummary;
using InventoryManagement.Application.Features.Reports.Queries.GetInventoryReport;
using InventoryManagement.Application.Features.Reports.Queries.GetProductMovementReport;
using InventoryManagement.Application.Features.Reports.Queries.GetTransactionReport;
using InventoryManagement.Application.Features.Products.Queries.GetAllProducts;
using InventoryManagement.Application.Features.Categories.Queries.GetAllCategories;
using InventoryManagement.Application.Features.Warehouses.Queries.GetAllWarehouses;
using InventoryManagement.Application.Features.Users.Queries.GetUsers;
using InventoryManagement.WebUI.ViewModels.Reports;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.WebUI.Controllers;

/// <summary>
/// Controller for reports and analytics
/// </summary>
[Authorize]
public class ReportsController : BaseController
{
    public ReportsController(IMediator mediator, ILogger<ReportsController> logger) 
        : base(mediator, logger)
    {
    }

    /// <summary>
    /// Display main dashboard with KPIs and charts
    /// </summary>
    /// <returns>Dashboard view</returns>
    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        try
        {
            var query = new GetDashboardSummaryQuery();
            var response = await _mediator.Send(query);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
                return View(new DashboardViewModel());
            }

            var viewModel = new DashboardViewModel
            {
                Summary = response.Summary,
                RefreshTime = DateTime.Now
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            TempData["ErrorMessage"] = "An error occurred while loading the dashboard.";
            return View(new DashboardViewModel());
        }
    }

    /// <summary>
    /// Display inventory report
    /// </summary>
    /// <param name="filter">Filter parameters</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="sortBy">Sort field</param>
    /// <param name="sortDirection">Sort direction</param>
    /// <returns>Inventory report view</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Inventory(
        InventoryReportFilterViewModel? filter = null,
        string? type = null,
        int pageNumber = 1,
        int pageSize = 25,
        string? sortBy = null,
        string? sortDirection = null)
    {
        try
        {
            filter ??= new InventoryReportFilterViewModel();

            // Handle type parameter from query string
            if (!string.IsNullOrEmpty(type))
            {
                switch (type.ToLower())
                {
                    case "lowstock":
                        filter.LowStockOnly = true;
                        break;
                    case "outofstock":
                        filter.OutOfStockOnly = true;
                        break;
                }
            }

            var reportFilter = new ReportFilterDto
            {
                ProductIds = filter.ProductIds,
                WarehouseIds = filter.WarehouseIds,
                CategoryIds = filter.CategoryIds,
                LowStockOnly = filter.LowStockOnly,
                OutOfStockOnly = filter.OutOfStockOnly,
                MinValue = filter.MinValue,
                MaxValue = filter.MaxValue
            };

            var query = new GetInventoryReportQuery
            {
                Filter = reportFilter,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDirection = sortDirection
            };

            var response = await _mediator.Send(query);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
                return View(new InventoryReportViewModel());
            }

            var viewModel = new InventoryReportViewModel
            {
                Items = response.Items,
                TotalCount = response.TotalCount,
                PageNumber = response.PageNumber,
                PageSize = response.PageSize,
                TotalPages = response.TotalPages,
                Summary = response.Summary,
                Filter = filter,
                SortBy = sortBy,
                SortDirection = sortDirection
            };

            // Load dropdown data after creating viewModel
            await LoadReportDropdownDataAsync(filter);

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading inventory report");
            TempData["ErrorMessage"] = "An error occurred while loading the inventory report.";
            
            // Still load dropdown data even when there's an error
            await LoadReportDropdownDataAsync(filter ?? new InventoryReportFilterViewModel());
            
            return View(new InventoryReportViewModel 
            { 
                Filter = filter ?? new InventoryReportFilterViewModel() 
            });
        }
    }

    /// <summary>
    /// Display transaction report
    /// </summary>
    /// <param name="filter">Filter parameters</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="sortBy">Sort field</param>
    /// <param name="sortDirection">Sort direction</param>
    /// <returns>Transaction report view</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Transactions(
        TransactionReportFilterViewModel? filter = null,
        int pageNumber = 1,
        int pageSize = 25,
        string? sortBy = null,
        string? sortDirection = null)
    {
        try
        {
            filter ??= new TransactionReportFilterViewModel();

            var reportFilter = new ReportFilterDto
            {
                StartDate = filter.StartDate,
                EndDate = filter.EndDate,
                ProductIds = filter.ProductIds,
                WarehouseIds = filter.WarehouseIds,
                UserIds = filter.UserIds,
                TransactionTypes = filter.TransactionTypes,
                MinValue = filter.MinValue,
                MaxValue = filter.MaxValue
            };

            var query = new GetTransactionReportQuery
            {
                Filter = reportFilter,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDirection = sortDirection
            };

            var response = await _mediator.Send(query);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
                return View(new TransactionReportViewModel());
            }

            var viewModel = new TransactionReportViewModel
            {
                Items = response.Items,
                TotalCount = response.TotalCount,
                PageNumber = response.PageNumber,
                PageSize = response.PageSize,
                TotalPages = response.TotalPages,
                Summary = response.Summary,
                Filter = filter,
                SortBy = sortBy,
                SortDirection = sortDirection
            };

            // Load dropdown data after creating viewModel
            await LoadReportDropdownDataAsync(filter);

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading transaction report");
            TempData["ErrorMessage"] = "An error occurred while loading the transaction report.";
            
            // Still load dropdown data even when there's an error
            await LoadReportDropdownDataAsync(filter ?? new TransactionReportFilterViewModel());
            
            return View(new TransactionReportViewModel 
            { 
                Filter = filter ?? new TransactionReportFilterViewModel() 
            });
        }
    }

    /// <summary>
    /// Display product movement report
    /// </summary>
    /// <param name="filter">Filter parameters</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="sortBy">Sort field</param>
    /// <param name="sortDirection">Sort direction</param>
    /// <returns>Product movement report view</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> ProductMovement(
        ProductMovementReportFilterViewModel? filter = null,
        int pageNumber = 1,
        int pageSize = 25,
        string? sortBy = null,
        string? sortDirection = null)
    {
        try
        {
            filter ??= new ProductMovementReportFilterViewModel();

            var reportFilter = new ReportFilterDto
            {
                StartDate = filter.StartDate,
                EndDate = filter.EndDate,
                ProductIds = filter.ProductIds,
                CategoryIds = filter.CategoryIds
            };

            var query = new GetProductMovementReportQuery
            {
                Filter = reportFilter,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDirection = sortDirection
            };

            var response = await _mediator.Send(query);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
                return View(new ProductMovementReportViewModel());
            }

            var viewModel = new ProductMovementReportViewModel
            {
                Items = response.Items,
                TotalCount = response.TotalCount,
                PageNumber = response.PageNumber,
                PageSize = response.PageSize,
                TotalPages = response.TotalPages,
                Filter = filter,
                SortBy = sortBy,
                SortDirection = sortDirection
            };

            // Load dropdown data after creating viewModel
            await LoadReportDropdownDataAsync(filter);

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading product movement report");
            TempData["ErrorMessage"] = "An error occurred while loading the product movement report.";
            
            // Still load dropdown data even when there's an error
            await LoadReportDropdownDataAsync(filter ?? new ProductMovementReportFilterViewModel());
            
            return View(new ProductMovementReportViewModel 
            { 
                Filter = filter ?? new ProductMovementReportFilterViewModel() 
            });
        }
    }

    /// <summary>
    /// Export inventory report to CSV
    /// </summary>
    /// <param name="filter">Filter parameters</param>
    /// <returns>CSV file download</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> ExportInventory([FromForm] InventoryReportFilterViewModel filter)
    {
        try
        {
            var reportFilter = new ReportFilterDto
            {
                ProductIds = filter.ProductIds,
                WarehouseIds = filter.WarehouseIds,
                CategoryIds = filter.CategoryIds,
                LowStockOnly = filter.LowStockOnly,
                OutOfStockOnly = filter.OutOfStockOnly,
                MinValue = filter.MinValue,
                MaxValue = filter.MaxValue
            };

            var query = new GetInventoryReportQuery
            {
                Filter = reportFilter,
                PageNumber = 1,
                PageSize = int.MaxValue // Get all records for export
            };

            var response = await _mediator.Send(query);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = "Failed to export inventory report.";
                return RedirectToAction(nameof(Inventory));
            }

            var csv = GenerateInventoryCsv(response.Items);
            var fileName = $"inventory_report_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";

            return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting inventory report");
            TempData["ErrorMessage"] = "An error occurred while exporting the inventory report.";
            return RedirectToAction(nameof(Inventory));
        }
    }

    /// <summary>
    /// Export transaction report to CSV
    /// </summary>
    /// <param name="filter">Filter parameters</param>
    /// <returns>CSV file download</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> ExportTransactions([FromForm] TransactionReportFilterViewModel filter)
    {
        try
        {
            var reportFilter = new ReportFilterDto
            {
                StartDate = filter.StartDate,
                EndDate = filter.EndDate,
                ProductIds = filter.ProductIds,
                WarehouseIds = filter.WarehouseIds,
                UserIds = filter.UserIds,
                TransactionTypes = filter.TransactionTypes,
                MinValue = filter.MinValue,
                MaxValue = filter.MaxValue
            };

            var query = new GetTransactionReportQuery
            {
                Filter = reportFilter,
                PageNumber = 1,
                PageSize = int.MaxValue // Get all records for export
            };

            var response = await _mediator.Send(query);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = "Failed to export transaction report.";
                return RedirectToAction(nameof(Transactions));
            }

            var csv = GenerateTransactionCsv(response.Items);
            var fileName = $"transaction_report_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";

            return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting transaction report");
            TempData["ErrorMessage"] = "An error occurred while exporting the transaction report.";
            return RedirectToAction(nameof(Transactions));
        }
    }

    /// <summary>
    /// Get dashboard data as JSON for AJAX calls
    /// </summary>
    /// <returns>JSON dashboard data</returns>
    [HttpGet]
    public async Task<IActionResult> GetDashboardData()
    {
        try
        {
            var query = new GetDashboardSummaryQuery();
            var response = await _mediator.Send(query);

            if (!response.Success)
            {
                return Json(new { success = false, message = response.ErrorMessage });
            }

            return Json(new { success = true, data = response.Summary });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard data");
            return Json(new { success = false, message = "An error occurred while loading dashboard data." });
        }
    }

    /// <summary>
    /// Load dropdown data for report filters
    /// </summary>
    private async Task LoadReportDropdownDataAsync<T>(T filter) where T : class
    {
        try
        {
            // Initialize empty lists in case of errors
            ViewBag.Products = new SelectList(new List<object>(), "Value", "Text");
            ViewBag.Categories = new SelectList(new List<object>(), "Value", "Text");
            ViewBag.Warehouses = new SelectList(new List<object>(), "Value", "Text");

            // Load Products
            try
            {
                var productsQuery = new GetAllProductsQuery 
                { 
                    PageNumber = 1, 
                    PageSize = 1000, 
                    ActiveOnly = true 
                };
                var productsResponse = await _mediator.Send(productsQuery);
                if (productsResponse?.Items != null)
                {
                    var products = productsResponse.Items.Select(p => new { Value = p.Id.ToString(), Text = $"{p.Name} - {p.SKU}" });
                    ViewBag.Products = new SelectList(products, "Value", "Text");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products for dropdown");
            }

            // Load Categories
            try
            {
                var categoriesQuery = new GetAllCategoriesQuery { ActiveOnly = true };
                var categories = await _mediator.Send(categoriesQuery);
                if (categories != null)
                {
                    var categoryList = categories.Select(c => new { Value = c.Id.ToString(), Text = c.Name });
                    ViewBag.Categories = new SelectList(categoryList, "Value", "Text");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories for dropdown");
            }

            // Load Warehouses
            try
            {
                var warehousesQuery = new GetAllWarehousesQuery 
                { 
                    PageNumber = 1, 
                    PageSize = 1000, 
                    ActiveOnly = true 
                };
                var warehousesResponse = await _mediator.Send(warehousesQuery);
                if (warehousesResponse?.Warehouses != null)
                {
                    var warehouses = warehousesResponse.Warehouses.Select(w => new { Value = w.Id.ToString(), Text = w.Name });
                    ViewBag.Warehouses = new SelectList(warehouses, "Value", "Text");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading warehouses for dropdown");
            }

            if (filter is TransactionReportFilterViewModel)
            {
                // Load Users
                try
                {
                    var usersQuery = new GetUsersQuery 
                    { 
                        Page = 1, 
                        PageSize = 1000, 
                        IsActive = true 
                    };
                    var usersResponse = await _mediator.Send(usersQuery);
                    if (usersResponse?.Users != null)
                    {
                        var users = usersResponse.Users.Select(u => new { Value = u.Id.ToString(), Text = u.FullName });
                        ViewBag.Users = new SelectList(users, "Value", "Text");
                    }
                    else
                    {
                        ViewBag.Users = new SelectList(new List<object>(), "Value", "Text");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading users for dropdown");
                    ViewBag.Users = new SelectList(new List<object>(), "Value", "Text");
                }

                ViewBag.TransactionTypes = new SelectList(new[]
                {
                    new { Value = "StockIn", Text = "Stock In" },
                    new { Value = "StockOut", Text = "Stock Out" },
                    new { Value = "Adjustment", Text = "Adjustment" }
                }, "Value", "Text");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dropdown data for reports");
        }
    }

    /// <summary>
    /// Generate CSV content for inventory report
    /// </summary>
    private string GenerateInventoryCsv(List<InventoryReportDto> items)
    {
        var csv = new System.Text.StringBuilder();
        
        // Header
        csv.AppendLine("Product Name,SKU,Category,Warehouse,Current Quantity,Reserved,Available,Min Level,Max Level,Unit Price,Total Value,Status");
        
        // Data rows
        foreach (var item in items)
        {
            csv.AppendLine($"\"{item.ProductName}\",\"{item.ProductSKU}\",\"{item.CategoryName}\",\"{item.WarehouseName}\"," +
                          $"{item.CurrentQuantity},{item.ReservedQuantity},{item.AvailableQuantity}," +
                          $"{item.MinimumStockLevel},{item.MaximumStockLevel},{item.UnitPrice:F2},{item.TotalValue:F2},\"{item.StockStatus}\"");
        }
        
        return csv.ToString();
    }

    /// <summary>
    /// Generate CSV content for transaction report
    /// </summary>
    private string GenerateTransactionCsv(List<TransactionReportDto> items)
    {
        var csv = new System.Text.StringBuilder();
        
        // Header
        csv.AppendLine("Date,Type,Product,SKU,Warehouse,Quantity Changed,Unit Cost,Total Value,User,Reference,Reason,Previous Qty,New Qty");
        
        // Data rows
        foreach (var item in items)
        {
            csv.AppendLine($"{item.TransactionDate:yyyy-MM-dd HH:mm:ss},\"{item.TransactionType}\",\"{item.ProductName}\"," +
                          $"\"{item.ProductSKU}\",\"{item.WarehouseName}\",{item.QuantityChanged},{item.UnitCost:F2}," +
                          $"{item.TotalValue:F2},\"{item.UserName}\",\"{item.ReferenceNumber}\",\"{item.Reason}\"," +
                          $"{item.PreviousQuantity},{item.NewQuantity}");
        }
        
        return csv.ToString();
    }
}
