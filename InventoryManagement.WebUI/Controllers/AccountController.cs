using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using InventoryManagement.WebUI.ViewModels;

namespace InventoryManagement.WebUI.Controllers;

/// <summary>
/// Controller for user authentication and account management
/// </summary>
public class AccountController : BaseController
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IMediator mediator,
        ILogger<AccountController> logger) : base(mediator, logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Display login page
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /// <summary>
    /// Process login attempt
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, 
                model.Password, 
                model.RememberMe, 
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} logged in successfully", model.Email);
                return RedirectToLocal(returnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, model.RememberMe });
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account {Email} locked out", model.Email);
                ModelState.AddModelError(string.Empty, "Account locked out due to multiple failed login attempts.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return View(model);
    }

    /// <summary>
    /// Display registration page
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /// <summary>
    /// Process registration attempt
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (ModelState.IsValid)
        {
            var user = new IdentityUser 
            { 
                UserName = model.Email, 
                Email = model.Email 
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} created a new account", model.Email);

                // Assign default role (Employee) to new users
                await _userManager.AddToRoleAsync(user, "Employee");

                // Sign in the user immediately after registration
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    /// <summary>
    /// Logout current user
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out");
        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Display access denied page
    /// </summary>
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    /// <summary>
    /// Display user profile page
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var model = new ProfileViewModel
        {
            Email = user.Email!,
            UserName = user.UserName!,
            Roles = await _userManager.GetRolesAsync(user)
        };

        return View(model);
    }

    /// <summary>
    /// Display change password page
    /// </summary>
    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword()
    {
        return View();
    }

    /// <summary>
    /// Process change password request
    /// </summary>
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User {Email} changed their password", user.Email);
            TempData["StatusMessage"] = "Your password has been changed successfully.";
            return RedirectToAction("Profile");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    /// <summary>
    /// User management page (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UserManagement()
    {
        var users = _userManager.Users.ToList();
        var userViewModels = new List<UserManagementViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userViewModels.Add(new UserManagementViewModel
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                Roles = roles,
                IsLockedOut = await _userManager.IsLockedOutAsync(user)
            });
        }

        return View(userViewModels);
    }

    /// <summary>
    /// Toggle user role
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var isInRole = await _userManager.IsInRoleAsync(user, role);
        
        if (isInRole)
        {
            await _userManager.RemoveFromRoleAsync(user, role);
        }
        else
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        return RedirectToAction("UserManagement");
    }

    #region Helper Methods

    /// <summary>
    /// Redirect to local URL or default action
    /// </summary>
    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Home");
    }

    #endregion
}
