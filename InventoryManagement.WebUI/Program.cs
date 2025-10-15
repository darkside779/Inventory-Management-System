using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MediatR;
using FluentValidation;
using InventoryManagement.Infrastructure.Persistence;
using InventoryManagement.Infrastructure.Persistence.Repositories;
using InventoryManagement.Infrastructure.Services;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Application.Mappings;
using InventoryManagement.Application.Behaviors;
using InventoryManagement.WebUI.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // Add global action filters
    options.Filters.Add<SecurityHeadersFilter>();
    options.Filters.Add<GlobalExceptionFilter>();
});

// Register action filters for dependency injection
builder.Services.AddScoped<AuditActionFilter>();
builder.Services.AddScoped<ValidateModelStateFilter>();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure Entity Framework
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.MigrationsAssembly("InventoryManagement.Infrastructure");
        sqlOptions.CommandTimeout(30);
    });
    
    // Enable sensitive data logging in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Register IApplicationDbContext interface
builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetService<AppDbContext>()!);

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile), typeof(InventoryManagement.WebUI.Mappings.ViewModelMappingProfile));

// Register MediatR with pipeline behaviors
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(MappingProfile).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(InventoryManagement.Application.Features.Payments.Commands.CreatePayment.CreatePaymentCommand).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
});

// Register FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(MappingProfile).Assembly);

// Register Repository Pattern services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerInvoiceRepository, CustomerInvoiceRepository>();
builder.Services.AddScoped<ICustomerPaymentRepository, CustomerPaymentRepository>();

// Register Identity Service
builder.Services.AddScoped<IIdentityService, IdentityService>();

// Configure Identity services
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    // Sign in settings
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

// Configure application cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Admin", "Manager"));
    options.AddPolicy("EmployeeAccess", policy => policy.RequireRole("Admin", "Manager", "Employee"));
});

var app = builder.Build();

// Initialize database
await InitializeDatabaseAsync(app);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

/// <summary>
/// Initialize database with migrations and seed data
/// </summary>
static async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        // Ensure database is created (for development)
        logger.LogInformation("Ensuring database is created...");
        var created = await context.Database.EnsureCreatedAsync();
        
        if (created)
        {
            logger.LogInformation("Database was created successfully.");
        }
        else
        {
            logger.LogInformation("Database already exists.");
        }
        
        // Seed initial data
        logger.LogInformation("Seeding database with initial data...");
        await DbSeeder.SeedAsync(context, logger);
        
        // Seed Identity data (roles and admin user)
        logger.LogInformation("Seeding Identity data...");
        await IdentitySeeder.SeedAsync(scope.ServiceProvider);
        
        logger.LogInformation("Database initialization completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database.");
        throw;
    }
}
