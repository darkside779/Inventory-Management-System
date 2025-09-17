using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using AutoMapper;
using InventoryManagement.Application.Features.Categories.Commands.CreateCategory;
using InventoryManagement.Application.Features.Categories.Commands.UpdateCategory;
using InventoryManagement.Application.Features.Categories.Commands.DeleteCategory;
using InventoryManagement.Application.Features.Categories.Queries.GetAllCategories;
using InventoryManagement.Application.Features.Categories.Queries.GetCategoryById;
using InventoryManagement.Application.DTOs;
using InventoryManagement.WebUI.ViewModels.Category;

namespace InventoryManagement.WebUI.Controllers;

/// <summary>
/// Controller for managing product categories
/// </summary>
[Authorize]
[Route("[controller]")]
public class CategoryController : BaseController
{
    private readonly IMapper _mapper;

    public CategoryController(IMediator mediator, IMapper mapper, ILogger<CategoryController> logger)
        : base(mediator, logger)
    {
        _mapper = mapper;
    }

    /// <summary>
    /// Display list of all categories
    /// </summary>
    [HttpGet]
    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            SetPageTitle("Categories", "Manage Product Categories");
            SetBreadcrumb(("Categories", null));

            var query = new GetAllCategoriesQuery { ActiveOnly = false };
            var categories = await _mediator.Send(query);
            var viewModel = new CategoryIndexViewModel
            {
                Categories = _mapper.Map<List<CategoryViewModel>>(categories),
                CanCreate = IsManagerOrAdmin,
                CanEdit = IsManagerOrAdmin,
                CanDelete = IsAdmin
            };

            _logger.LogInformation("Retrieved {Count} categories for display", viewModel.Categories.Count);
            LogUserAction("Viewed Categories List");
            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading categories");
        }
    }

    /// <summary>
    /// Display create category form
    /// </summary>
    [HttpGet("Create")]
    [Authorize(Roles = "Administrator,Manager")]
    public IActionResult Create()
    {
        SetPageTitle("Create Category", "Add New Category");
        SetBreadcrumb(("Categories", Url.Action("Index")), ("Create", null));

        var viewModel = new CreateCategoryViewModel();
        return View(viewModel);
    }

    /// <summary>
    /// Process create category form submission
    /// </summary>
    [HttpPost("Create")]
    [Authorize(Roles = "Administrator,Manager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCategoryViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            SetPageTitle("Create Category", "Add New Category");
            SetBreadcrumb(("Categories", Url.Action("Index")), ("Create", null));
            return View(viewModel);
        }

        try
        {
            var createDto = _mapper.Map<CreateCategoryDto>(viewModel);
            var command = new CreateCategoryCommand { CategoryDto = createDto };
            var result = await _mediator.Send(command);

            _logger.LogInformation("Successfully created category with ID: {CategoryId}", result.Id);
            LogUserAction($"Created category: {result.Name}");

            TempData["SuccessMessage"] = $"Category '{result.Name}' created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            SetPageTitle("Create Category", "Add New Category");
            SetBreadcrumb(("Categories", Url.Action("Index")), ("Create", null));
            return View(viewModel);
        }
    }

    /// <summary>
    /// Display edit category form
    /// </summary>
    [HttpGet("Edit/{id:int}")]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var query = new GetCategoryByIdQuery { Id = id };
            var category = await _mediator.Send(query);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            SetPageTitle("Edit Category", $"Edit {category.Name}");
            SetBreadcrumb(("Categories", Url.Action("Index")), ("Edit", null));

            var viewModel = _mapper.Map<EditCategoryViewModel>(category);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading category for editing");
        }
    }

    /// <summary>
    /// Process edit category form submission
    /// </summary>
    [HttpPost("Edit/{id:int}")]
    [Authorize(Roles = "Administrator,Manager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditCategoryViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            return BadRequest("Category ID mismatch.");
        }

        if (!ModelState.IsValid)
        {
            SetPageTitle("Edit Category", $"Edit {viewModel.Name}");
            SetBreadcrumb(("Categories", Url.Action("Index")), ("Edit", null));
            return View(viewModel);
        }

        try
        {
            var updateDto = _mapper.Map<UpdateCategoryDto>(viewModel);
            var command = new UpdateCategoryCommand { CategoryDto = updateDto };
            var result = await _mediator.Send(command);

            _logger.LogInformation("Successfully updated category with ID: {CategoryId}", result.Id);
            LogUserAction($"Updated category: {result.Name}");

            TempData["SuccessMessage"] = $"Category '{result.Name}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            SetPageTitle("Edit Category", $"Edit {viewModel.Name}");
            SetBreadcrumb(("Categories", Url.Action("Index")), ("Edit", null));
            return View(viewModel);
        }
    }

    /// <summary>
    /// Display category details
    /// </summary>
    [HttpGet("Details/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var query = new GetCategoryByIdQuery { Id = id };
            var category = await _mediator.Send(query);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            SetPageTitle("Category Details", category.Name);
            SetBreadcrumb(("Categories", Url.Action("Index")), ("Details", null));

            var viewModel = _mapper.Map<CategoryDetailsViewModel>(category);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading category details");
        }
    }

    /// <summary>
    /// Display delete category confirmation
    /// </summary>
    [HttpGet("Delete/{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var query = new GetCategoryByIdQuery { Id = id };
            var category = await _mediator.Send(query);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            SetPageTitle("Delete Category", $"Delete {category.Name}");
            SetBreadcrumb(("Categories", Url.Action("Index")), ("Delete", null));

            var viewModel = _mapper.Map<DeleteCategoryViewModel>(category);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "loading category for deletion");
        }
    }

    /// <summary>
    /// Process delete category confirmation
    /// </summary>
    [HttpPost("Delete/{id:int}")]
    [Authorize(Roles = "Administrator")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var command = new DeleteCategoryCommand { Id = id };
            await _mediator.Send(command);

            _logger.LogInformation("Successfully deleted category with ID: {CategoryId}", id);
            LogUserAction($"Deleted category with ID: {id}");

            TempData["SuccessMessage"] = "Category deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Delete), new { id });
        }
    }
}
