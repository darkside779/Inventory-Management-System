using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MediatR;
using InventoryManagement.WebUI.ViewModels.Users;
using InventoryManagement.Application.Features.Users.Commands.CreateUser;
using InventoryManagement.Application.Features.Users.Commands.UpdateUser;
using InventoryManagement.Application.Features.Users.Commands.DeleteUser;
using InventoryManagement.Application.Features.Users.Commands.ChangePassword;
using InventoryManagement.Application.Features.Users.Queries.GetUsers;
using InventoryManagement.Application.Features.Users.Queries.GetUserById;
using InventoryManagement.Application.Features.Users.Queries.GetUserStats;
using InventoryManagement.Domain.Enums;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.WebUI.Controllers;

/// <summary>
/// Controller for user management operations
/// </summary>
[Authorize(Roles = "Administrator,Manager")]
public class UserController : BaseController
{
    public UserController(IMediator mediator, ILogger<UserController> logger) 
        : base(mediator, logger)
    {
    }

    /// <summary>
    /// Display list of users with filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(UserFilterViewModel filter)
    {
        try
        {
            _logger.LogInformation("Loading users list with filters");

            var query = new GetUsersQuery
            {
                SearchTerm = filter.SearchTerm,
                Role = filter.Role,
                IsActive = filter.IsActive ?? true, // Default to showing only active users
                CreatedFrom = filter.CreatedFrom,
                CreatedTo = filter.CreatedTo,
                HasRecentLogin = filter.HasRecentLogin,
                Page = filter.Page,
                PageSize = filter.PageSize,
                SortBy = filter.SortBy,
                SortDirection = filter.SortDirection
            };

            var result = await _mediator.Send(query);

            var viewModel = new UserIndexViewModel
            {
                Users = new PagedResult<UserDto>
                {
                    Items = result.Users,
                    TotalCount = result.TotalCount,
                    Page = result.Page,
                    PageSize = result.PageSize
                },
                Filter = filter,
                RoleOptions = GetRoleSelectList(),
                CurrentSort = filter.SortBy,
                SortDirection = filter.SortDirection
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users list");
            SetErrorMessage("Failed to load users. Please try again.");
            return View(new UserIndexViewModel());
        }
    }

    /// <summary>
    /// Display user details
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            _logger.LogInformation("Loading user details for ID: {UserId}", id);

            var userQuery = new GetUserByIdQuery(id);
            var user = await _mediator.Send(userQuery);

            if (user == null)
            {
                SetErrorMessage("User not found.");
                return RedirectToAction(nameof(Index));
            }

            var statsQuery = new GetUserStatsQuery(id);
            var stats = await _mediator.Send(statsQuery);

            var viewModel = new UserDetailsViewModel
            {
                User = user,
                Stats = new UserStatsViewModel
                {
                    TotalTransactions = stats.TotalTransactions,
                    TransactionsLast30Days = stats.TransactionsLast30Days,
                    TotalTransactionValue = stats.TotalTransactionValue,
                    DaysSinceLastLogin = stats.DaysSinceLastLogin,
                    AccountAgeInDays = stats.AccountAgeInDays,
                    LoginFrequency = stats.LoginFrequency
                },
                RecentActivities = new List<UserActivityViewModel>(), // TODO: Implement activity tracking
                CanEdit = CanEditUser(user),
                CanDelete = CanDeleteUser(user),
                CanResetPassword = CanResetPassword(user)
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user details for ID: {UserId}", id);
            SetErrorMessage("Failed to load user details. Please try again.");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Display create user form
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public IActionResult Create()
    {
        var viewModel = new CreateUserViewModel
        {
            RoleOptions = GetRoleSelectList(),
            IsActive = true,
            SendWelcomeEmail = true
        };

        return View(viewModel);
    }

    /// <summary>
    /// Handle create user form submission
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                model.RoleOptions = GetRoleSelectList();
                return View(model);
            }

            _logger.LogInformation("Creating new user with username: {Username}", model.Username);

            var command = new CreateUserCommand
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Role = model.Role,
                IsActive = model.IsActive,
                SendWelcomeEmail = model.SendWelcomeEmail
            };

            var result = await _mediator.Send(command);

            SetSuccessMessage($"User '{result.Username}' created successfully.");
            return RedirectToAction(nameof(Details), new { id = result.Id });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            model.RoleOptions = GetRoleSelectList();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with username: {Username}", model.Username);
            SetErrorMessage("Failed to create user. Please try again.");
            model.RoleOptions = GetRoleSelectList();
            return View(model);
        }
    }

    /// <summary>
    /// Display edit user form
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            _logger.LogInformation("Loading edit form for user ID: {UserId}", id);

            var query = new GetUserByIdQuery(id);
            var user = await _mediator.Send(query);

            if (user == null)
            {
                SetErrorMessage("User not found.");
                return RedirectToAction(nameof(Index));
            }

            if (!CanEditUser(user))
            {
                SetErrorMessage("You don't have permission to edit this user.");
                return RedirectToAction(nameof(Details), new { id });
            }

            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                RoleOptions = GetRoleSelectList(),
                CanEdit = CanEditUser(user),
                CanDelete = CanDeleteUser(user)
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading edit form for user ID: {UserId}", id);
            SetErrorMessage("Failed to load user information. Please try again.");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Handle edit user form submission
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditUserViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                model.RoleOptions = GetRoleSelectList();
                return View(model);
            }

            // Get current user to check permissions
            var currentUser = await _mediator.Send(new GetUserByIdQuery(model.Id));
            if (currentUser == null)
            {
                SetErrorMessage("User not found.");
                return RedirectToAction(nameof(Index));
            }

            if (!CanEditUser(currentUser))
            {
                SetErrorMessage("You don't have permission to edit this user.");
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }

            _logger.LogInformation("Updating user with ID: {UserId}", model.Id);

            var command = new UpdateUserCommand
            {
                Id = model.Id,
                Username = model.Username,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Role = model.Role,
                IsActive = model.IsActive
            };

            var result = await _mediator.Send(command);

            SetSuccessMessage($"User '{result.Username}' updated successfully.");
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            model.RoleOptions = GetRoleSelectList();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID: {UserId}", model.Id);
            SetErrorMessage("Failed to update user. Please try again.");
            model.RoleOptions = GetRoleSelectList();
            return View(model);
        }
    }

    /// <summary>
    /// Display delete confirmation
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var query = new GetUserByIdQuery(id);
            var user = await _mediator.Send(query);

            if (user == null)
            {
                SetErrorMessage("User not found.");
                return RedirectToAction(nameof(Index));
            }

            if (!CanDeleteUser(user))
            {
                SetErrorMessage("You cannot delete this user.");
                return RedirectToAction(nameof(Details), new { id });
            }

            // Create ViewModel for Delete view
            var viewModel = new ViewModels.User.UserDetailsViewModel
            {
                Id = user.Id.ToString(),
                UserName = user.Username,
                Email = user.Email,
                FullName = user.FirstName + " " + user.LastName,
                PhoneNumber = "", // Not available in UserDto
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                CreatedDate = user.CreatedAt,
                LastLoginDate = user.LastLoginAt,
                RecentActivities = new List<ViewModels.User.UserActivityViewModel>()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading delete confirmation for user ID: {UserId}", id);
            SetErrorMessage("Failed to load user information. Please try again.");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Handle user deletion
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var user = await _mediator.Send(new GetUserByIdQuery(id));
            if (user == null)
            {
                SetErrorMessage("User not found.");
                return RedirectToAction(nameof(Index));
            }

            if (!CanDeleteUser(user))
            {
                SetErrorMessage("You cannot delete this user.");
                return RedirectToAction(nameof(Details), new { id });
            }

            _logger.LogInformation("Deleting user with ID: {UserId}", id);

            var command = new DeleteUserCommand { Id = id, HardDelete = false }; // Soft delete by default
            var result = await _mediator.Send(command);

            if (result)
            {
                SetSuccessMessage($"User '{user.Username}' has been deactivated.");
            }
            else
            {
                SetErrorMessage("Failed to delete user.");
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID: {UserId}", id);
            SetErrorMessage("Failed to delete user. Please try again.");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Display change password form
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ChangePassword(int id)
    {
        try
        {
            var query = new GetUserByIdQuery(id);
            var user = await _mediator.Send(query);

            if (user == null)
            {
                SetErrorMessage("User not found.");
                return RedirectToAction(nameof(Index));
            }

            if (!CanResetPassword(user))
            {
                SetErrorMessage("You don't have permission to change this user's password.");
                return RedirectToAction(nameof(Details), new { id });
            }

            var currentUserId = GetCurrentUserIdAsInt();
            var isAdminReset = currentUserId != id && IsAdministrator();

            var viewModel = new ChangePasswordViewModel
            {
                UserId = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                IsAdminReset = isAdminReset,
                SendNotification = true
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading change password form for user ID: {UserId}", id);
            SetErrorMessage("Failed to load password change form. Please try again.");
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    /// <summary>
    /// Handle password change form submission
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _mediator.Send(new GetUserByIdQuery(model.UserId));
            if (user == null)
            {
                SetErrorMessage("User not found.");
                return RedirectToAction(nameof(Index));
            }

            if (!CanResetPassword(user))
            {
                SetErrorMessage("You don't have permission to change this user's password.");
                return RedirectToAction(nameof(Details), new { id = model.UserId });
            }

            _logger.LogInformation("Changing password for user ID: {UserId}", model.UserId);

            var command = new ChangePasswordCommand
            {
                UserId = model.UserId,
                CurrentPassword = model.CurrentPassword,
                NewPassword = model.NewPassword,
                IsAdminReset = model.IsAdminReset,
                ChangedByUserId = GetCurrentUserIdAsInt()
            };

            var result = await _mediator.Send(command);

            if (result)
            {
                SetSuccessMessage("Password changed successfully.");
                return RedirectToAction(nameof(Details), new { id = model.UserId });
            }

            SetErrorMessage("Failed to change password.");
            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user ID: {UserId}", model.UserId);
            SetErrorMessage("Failed to change password. Please try again.");
            return View(model);
        }
    }

    /// <summary>
    /// Display user profile form
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Profile()
    {
        try
        {
            var currentUserId = GetCurrentUserIdAsInt();
            if (currentUserId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var query = new GetUserByIdQuery(currentUserId);
            var user = await _mediator.Send(query);

            if (user == null)
            {
                SetErrorMessage("User profile not found.");
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new UserProfileViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user profile");
            SetErrorMessage("Failed to load profile. Please try again.");
            return RedirectToAction("Index", "Home");
        }
    }

    /// <summary>
    /// Handle profile update
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Profile(UserProfileViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUserId = GetCurrentUserIdAsInt();
            if (currentUserId != model.Id)
            {
                SetErrorMessage("You can only update your own profile.");
                return View(model);
            }

            _logger.LogInformation("Updating profile for user ID: {UserId}", model.Id);

            var user = await _mediator.Send(new GetUserByIdQuery(model.Id));
            if (user == null)
            {
                SetErrorMessage("User profile not found.");
                return View(model);
            }

            var command = new UpdateUserCommand
            {
                Id = model.Id,
                Username = user.Username, // Keep existing username
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Role = user.Role, // Keep existing role
                IsActive = user.IsActive // Keep existing status
            };

            await _mediator.Send(command);

            SetSuccessMessage("Profile updated successfully.");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user ID: {UserId}", model.Id);
            SetErrorMessage("Failed to update profile. Please try again.");
            return View(model);
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Get role options for dropdown lists
    /// </summary>
    private SelectList GetRoleSelectList()
    {
        var roles = Enum.GetValues<UserRole>()
            .Select(r => new SelectListItem
            {
                Value = ((int)r).ToString(),
                Text = r.ToString()
            })
            .ToList();

        return new SelectList(roles, "Value", "Text");
    }

    /// <summary>
    /// Check if current user can edit the specified user
    /// </summary>
    private bool CanEditUser(UserDto user)
    {
        var currentUserId = GetCurrentUserIdAsInt();
        
        // Users can edit their own profile (except role changes)
        if (currentUserId == user.Id)
            return true;
            
        // Administrators can edit anyone
        if (IsAdministrator())
            return true;
            
        // Managers can edit staff members
        if (IsManager() && user.Role == UserRole.Staff)
            return true;

        return false;
    }

    /// <summary>
    /// Check if current user can delete the specified user
    /// </summary>
    private bool CanDeleteUser(UserDto user)
    {
        var currentUserId = GetCurrentUserIdAsInt();
        
        // Users cannot delete themselves
        if (currentUserId == user.Id)
            return false;
            
        // Only administrators can delete users
        return IsAdministrator();
    }

    /// <summary>
    /// Check if current user can reset password for the specified user
    /// </summary>
    private bool CanResetPassword(UserDto user)
    {
        var currentUserId = GetCurrentUserIdAsInt();
        
        // Users can change their own password
        if (currentUserId == user.Id)
            return true;
            
        // Administrators can reset anyone's password
        if (IsAdministrator())
            return true;
            
        // Managers can reset staff passwords
        if (IsManager() && user.Role == UserRole.Staff)
            return true;

        return false;
    }

    /// <summary>
    /// Get current user ID from claims - overrides base implementation for user management
    /// </summary>
    private new string GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("UserId");
        return userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId) ? userId.ToString() : "0";
    }

    /// <summary>
    /// Check if current user is Administrator
    /// </summary>
    private bool IsAdministrator()
    {
        return User.IsInRole("Administrator");
    }

    /// <summary>
    /// Check if current user is Manager
    /// </summary>
    private bool IsManager()
    {
        return User.IsInRole("Manager");
    }

    #endregion
}
