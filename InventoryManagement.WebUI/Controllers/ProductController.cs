using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using AutoMapper;
using InventoryManagement.Application.Features.Products.Queries.GetAllProducts;
using InventoryManagement.Application.Features.Products.Queries.GetProductById;
using InventoryManagement.Application.Features.Products.Commands.CreateProduct;
using InventoryManagement.Application.Features.Products.Commands.UpdateProduct;
using InventoryManagement.Application.Features.Products.Commands.DeleteProduct;
using InventoryManagement.Application.DTOs;
using InventoryManagement.WebUI.ViewModels.Products;
using InventoryManagement.WebUI.Controllers;

namespace InventoryManagement.WebUI.Controllers;

/// <summary>
/// Controller for product management operations
/// </summary>
[Authorize]
public class ProductController : BaseController
{
    private readonly IMapper _mapper;

    public ProductController(IMediator mediator, IMapper mapper, ILogger<ProductController> logger)
        : base(mediator, logger)
    {
        _mapper = mapper;
    }

    /// <summary>
    /// Display paginated list of products
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, int? categoryId = null)
    {
        try
        {
            LogUserAction("Viewed Products List", $"Page: {page}, Search: {search}");

            var query = new GetAllProductsQuery
            {
                PageNumber = page,
                PageSize = pageSize,
                SearchTerm = search,
                CategoryId = categoryId,
                ActiveOnly = true
            };

            var result = await _mediator.Send(query);
            var viewModel = _mapper.Map<ProductIndexViewModel>(result);
            
            // Set search parameters for view
            viewModel.CurrentSearch = search;
            viewModel.CurrentCategoryId = categoryId;

            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading products list");
        }
    }

    /// <summary>
    /// Display product details
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            LogUserAction("Viewed Product Details", $"Product ID: {id}");

            var query = new GetProductByIdQuery(id);
            var productDto = await _mediator.Send(query);

            if (productDto == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            var viewModel = _mapper.Map<ProductDetailsViewModel>(productDto);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading product details");
        }
    }

    /// <summary>
    /// Display create product form
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Manager,Administrator")]
    public IActionResult Create()
    {
        try
        {
            LogUserAction("Accessed Product Creation Form");

            var viewModel = new CreateProductViewModel();
            PopulateDropdowns(viewModel);
            
            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading product creation form");
        }
    }

    /// <summary>
    /// Handle product creation
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Manager,Administrator")]
    public async Task<IActionResult> Create(CreateProductViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model);
                return View(model);
            }

            var createDto = _mapper.Map<CreateProductDto>(model);
            var command = new CreateProductCommand { ProductDto = createDto };
            
            var result = await _mediator.Send(command);
            
            LogUserAction("Created Product", $"Product: {result.Name}, SKU: {result.SKU}");
            
            TempData["SuccessMessage"] = $"Product '{result.Name}' created successfully.";
            return RedirectToAction(nameof(Details), new { id = result.Id });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            PopulateDropdowns(model);
            return View(model);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "creating product");
        }
    }

    /// <summary>
    /// Display edit product form
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Manager,Administrator")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            LogUserAction("Accessed Product Edit Form", $"Product ID: {id}");

            var query = new GetProductByIdQuery(id);
            var productDto = await _mediator.Send(query);

            if (productDto == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            var viewModel = _mapper.Map<EditProductViewModel>(productDto);
            PopulateDropdowns(viewModel);
            
            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading product edit form");
        }
    }

    /// <summary>
    /// Handle product update
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Manager,Administrator")]
    public async Task<IActionResult> Edit(int id, EditProductViewModel model)
    {
        try
        {
            if (id != model.Id)
            {
                return BadRequest("Product ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model);
                return View(model);
            }

            var updateDto = _mapper.Map<UpdateProductDto>(model);
            var command = new UpdateProductCommand { Id = id, ProductDto = updateDto };
            
            var result = await _mediator.Send(command);
            
            LogUserAction("Updated Product", $"Product: {result.Name}, SKU: {result.SKU}");
            
            TempData["SuccessMessage"] = $"Product '{result.Name}' updated successfully.";
            return RedirectToAction(nameof(Details), new { id = result.Id });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            PopulateDropdowns(model);
            return View(model);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "updating product");
        }
    }

    /// <summary>
    /// Display delete confirmation
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Manager,Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            LogUserAction("Accessed Product Delete Confirmation", $"Product ID: {id}");

            var query = new GetProductByIdQuery(id);
            var productDto = await _mediator.Send(query);

            if (productDto == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            var viewModel = _mapper.Map<DeleteProductViewModel>(productDto);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading product delete confirmation");
        }
    }

    /// <summary>
    /// Handle product deletion
    /// </summary>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Manager,Administrator")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var command = new DeleteProductCommand(id);
            var result = await _mediator.Send(command);

            if (result)
            {
                LogUserAction("Deleted Product", $"Product ID: {id}");
                TempData["SuccessMessage"] = "Product deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Product not found or could not be deleted.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            return HandleException(ex, "deleting product");
        }
    }

    /// <summary>
    /// API endpoint to get products for AJAX requests
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetProducts(int page = 1, int pageSize = 10, string? search = null, int? categoryId = null)
    {
        try
        {
            var query = new GetAllProductsQuery
            {
                PageNumber = page,
                PageSize = pageSize,
                SearchTerm = search,
                CategoryId = categoryId,
                ActiveOnly = true
            };

            var result = await _mediator.Send(query);
            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products via API");
            return BadRequest("Error retrieving products");
        }
    }

    /// <summary>
    /// API endpoint to check if SKU exists
    /// </summary>
    [HttpGet]
    public IActionResult CheckSkuExists(string sku, int? excludeId = null)
    {
        try
        {
            // This would need to be implemented in the application layer
            // For now, return false
            return Json(new { exists = false });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking SKU existence");
            return BadRequest("Error checking SKU");
        }
    }

    /// <summary>
    /// Populate dropdown lists for create/edit forms
    /// </summary>
    private void PopulateDropdowns(dynamic viewModel)
    {
        // This would need category and supplier queries to be implemented
        // For now, create empty lists
        viewModel.Categories = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
        viewModel.Suppliers = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
    }
}
