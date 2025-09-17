using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using InventoryManagement.WebUI.Models;

namespace InventoryManagement.WebUI.Controllers;

public class HomeController : BaseController
{
    public HomeController(IMediator mediator, ILogger<HomeController> logger) 
        : base(mediator, logger)
    {
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        // If user is authenticated, redirect to Dashboard
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }
        
        // For anonymous users, show welcome page
        SetPageTitle("Welcome", "Inventory Management System");
        SetBreadcrumb(("Home", null));
        
        return View();
    }

    public IActionResult Privacy()
    {
        SetPageTitle("Privacy Policy");
        SetBreadcrumb(("Privacy", null));
        
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
