using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Infrastructure.Persistence;

/// <summary>
/// Database seeder for initial data
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Seed the database with initial data
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="logger">Logger instance</param>
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        try
        {
            logger.LogInformation("Starting database seeding...");
            
            // Seed Categories
            await SeedCategoriesAsync(context, logger);
            
            // Seed Warehouses
            await SeedWarehousesAsync(context, logger);
            
            // Seed Suppliers
            await SeedSuppliersAsync(context, logger);
            
            // Seed Admin User
            await SeedUsersAsync(context, logger);
            
            // Seed Products (after categories and suppliers)
            await SeedProductsAsync(context, logger);
            
            // Seed Initial Inventory
            await SeedInventoryAsync(context, logger);
            
            // Save all changes
            await context.SaveChangesAsync();
            
            logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    /// <summary>
    /// Seed categories
    /// </summary>
    private static async Task SeedCategoriesAsync(AppDbContext context, ILogger logger)
    {
        if (await context.Categories.AnyAsync())
        {
            logger.LogInformation("Categories already exist, skipping seed.");
            return;
        }

        var categories = new[]
        {
            new Category { Name = "Electronics", Description = "Electronic devices and components" },
            new Category { Name = "Clothing", Description = "Apparel and fashion items" },
            new Category { Name = "Books", Description = "Books and educational materials" },
            new Category { Name = "Home & Garden", Description = "Home improvement and garden supplies" },
            new Category { Name = "Sports & Outdoors", Description = "Sports equipment and outdoor gear" },
            new Category { Name = "Automotive", Description = "Car parts and automotive supplies" },
            new Category { Name = "Health & Beauty", Description = "Health, beauty, and personal care products" },
            new Category { Name = "Office Supplies", Description = "Office equipment and stationery" },
            new Category { Name = "Tools & Hardware", Description = "Tools and hardware supplies" },
            new Category { Name = "Food & Beverages", Description = "Food items and beverages" }
        };

        context.Categories.AddRange(categories);
        logger.LogInformation("Added {Count} categories.", categories.Length);
    }

    /// <summary>
    /// Seed warehouses
    /// </summary>
    private static async Task SeedWarehousesAsync(AppDbContext context, ILogger logger)
    {
        if (await context.Warehouses.AnyAsync())
        {
            logger.LogInformation("Warehouses already exist, skipping seed.");
            return;
        }

        var warehouses = new[]
        {
            new Warehouse 
            { 
                Name = "Main Warehouse", 
                Location = "Downtown", 
                Address = "123 Main Street, City Center, State 12345",
                ContactPhone = "+1-555-0101",
                ContactEmail = "main@warehouse.com",
                Capacity = 10000
            },
            new Warehouse 
            { 
                Name = "North Distribution Center", 
                Location = "North District", 
                Address = "456 North Avenue, North District, State 12346",
                ContactPhone = "+1-555-0102",
                ContactEmail = "north@warehouse.com",
                Capacity = 7500
            },
            new Warehouse 
            { 
                Name = "South Storage Facility", 
                Location = "South District", 
                Address = "789 South Boulevard, South District, State 12347",
                ContactPhone = "+1-555-0103",
                ContactEmail = "south@warehouse.com",
                Capacity = 5000
            },
            new Warehouse 
            { 
                Name = "East Regional Hub", 
                Location = "East Side", 
                Address = "321 East Road, East Side, State 12348",
                ContactPhone = "+1-555-0104",
                ContactEmail = "east@warehouse.com",
                Capacity = 8000
            },
            new Warehouse 
            { 
                Name = "West Coast Depot", 
                Location = "West District", 
                Address = "654 West Street, West District, State 12349",
                ContactPhone = "+1-555-0105",
                ContactEmail = "west@warehouse.com",
                Capacity = 6000
            }
        };

        context.Warehouses.AddRange(warehouses);
        logger.LogInformation("Added {Count} warehouses.", warehouses.Length);
    }

    /// <summary>
    /// Seed suppliers
    /// </summary>
    private static async Task SeedSuppliersAsync(AppDbContext context, ILogger logger)
    {
        if (await context.Suppliers.AnyAsync())
        {
            logger.LogInformation("Suppliers already exist, skipping seed.");
            return;
        }

        var suppliers = new[]
        {
            new Supplier 
            { 
                Name = "TechSupplier Co.", 
                ContactInfo = "John Smith, Purchasing Manager",
                Address = "100 Tech Park, Silicon Valley, CA 94000",
                Phone = "+1-800-TECH-001",
                Email = "orders@techsupplier.com",
                Website = "www.techsupplier.com",
                PaymentTerms = "Net 30"
            },
            new Supplier 
            { 
                Name = "Fashion Forward Ltd.", 
                ContactInfo = "Jane Doe, Sales Director",
                Address = "200 Fashion Ave, New York, NY 10001",
                Phone = "+1-800-FASHION-1",
                Email = "sales@fashionforward.com",
                Website = "www.fashionforward.com",
                PaymentTerms = "Net 15"
            },
            new Supplier 
            { 
                Name = "Book Distributors Inc.", 
                ContactInfo = "Mike Johnson, Distribution Head",
                Address = "300 Library Lane, Boston, MA 02101",
                Phone = "+1-800-BOOKS-01",
                Email = "orders@bookdist.com",
                Website = "www.bookdistributors.com",
                PaymentTerms = "Net 45"
            },
            new Supplier 
            { 
                Name = "Home & Garden Supply", 
                ContactInfo = "Sarah Wilson, Account Manager",
                Address = "400 Garden Road, Portland, OR 97201",
                Phone = "+1-800-GARDEN-1",
                Email = "wholesale@homegardens.com",
                Website = "www.homegardens.com",
                PaymentTerms = "Net 30"
            },
            new Supplier 
            { 
                Name = "Global Office Solutions", 
                ContactInfo = "David Lee, B2B Sales",
                Address = "700 Corporate Center, Chicago, IL 60601",
                Phone = "+1-800-OFFICE-1",
                Email = "b2b@globaloffice.com",
                Website = "www.globalofficesolutions.com",
                PaymentTerms = "Net 30"
            }
        };

        context.Suppliers.AddRange(suppliers);
        logger.LogInformation("Added {Count} suppliers.", suppliers.Length);
    }

    /// <summary>
    /// Seed users
    /// </summary>
    private static async Task SeedUsersAsync(AppDbContext context, ILogger logger)
    {
        if (await context.Users.AnyAsync())
        {
            logger.LogInformation("Users already exist, skipping seed.");
            return;
        }

        // Note: In production, use proper password hashing
        // This is a simplified hash for demonstration
        var users = new[]
        {
            new User 
            { 
                Username = "admin", 
                Email = "admin@inventoryms.com",
                PasswordHash = "AQAAAAEAACcQAAAAEJ4/WHJhBvs3G7l4Zl5xXy+wLj3QWgTqrKdWgjKEPQaJNQx8eLnmZPzXGWKKYjGYZw==", // Admin@123
                FullName = "System Administrator",
                Role = UserRole.Administrator,
                PhoneNumber = "+1-555-0001"
            },
            new User 
            { 
                Username = "manager1", 
                Email = "manager1@inventoryms.com",
                PasswordHash = "AQAAAAEAACcQAAAAEJ4/WHJhBvs3G7l4Zl5xXy+wLj3QWgTqrKdWgjKEPQaJNQx8eLnmZPzXGWKKYjGYZw==", // Manager@123
                FullName = "John Manager",
                Role = UserRole.Manager,
                PhoneNumber = "+1-555-0002"
            },
            new User 
            { 
                Username = "staff1", 
                Email = "staff1@inventoryms.com",
                PasswordHash = "AQAAAAEAACcQAAAAEJ4/WHJhBvs3G7l4Zl5xXy+wLj3QWgTqrKdWgjKEPQaJNQx8eLnmZPzXGWKKYjGYZw==", // Staff@123
                FullName = "Bob Staff",
                Role = UserRole.Staff,
                PhoneNumber = "+1-555-0004"
            }
        };

        context.Users.AddRange(users);
        logger.LogInformation("Added {Count} users.", users.Length);
        logger.LogInformation("Default login credentials:");
        logger.LogInformation("Admin: admin / Admin@123");
        logger.LogInformation("Manager: manager1 / Manager@123");
        logger.LogInformation("Staff: staff1 / Staff@123");
    }

    /// <summary>
    /// Seed products
    /// </summary>
    private static async Task SeedProductsAsync(AppDbContext context, ILogger logger)
    {
        if (await context.Products.AnyAsync())
        {
            logger.LogInformation("Products already exist, skipping seed.");
            return;
        }

        // Wait for categories and suppliers to be saved
        await context.SaveChangesAsync();

        var electronicsCategory = await context.Categories.FirstAsync(c => c.Name == "Electronics");
        var clothingCategory = await context.Categories.FirstAsync(c => c.Name == "Clothing");
        var booksCategory = await context.Categories.FirstAsync(c => c.Name == "Books");
        var officeCategory = await context.Categories.FirstAsync(c => c.Name == "Office Supplies");
        
        var techSupplier = await context.Suppliers.FirstAsync(s => s.Name == "TechSupplier Co.");
        var fashionSupplier = await context.Suppliers.FirstAsync(s => s.Name == "Fashion Forward Ltd.");
        var bookSupplier = await context.Suppliers.FirstAsync(s => s.Name == "Book Distributors Inc.");
        var officeSupplier = await context.Suppliers.FirstAsync(s => s.Name == "Global Office Solutions");

        var products = new[]
        {
            // Electronics
            new Product 
            { 
                Name = "Wireless Mouse", 
                SKU = "ELEC-WM-001",
                Description = "Ergonomic wireless mouse with USB receiver",
                Price = 29.99m,
                Cost = 18.50m,
                CategoryId = electronicsCategory.Id,
                SupplierId = techSupplier.Id,
                LowStockThreshold = 20,
                Unit = "Piece",
                Barcode = "1234567890123"
            },
            new Product 
            { 
                Name = "Bluetooth Keyboard", 
                SKU = "ELEC-KB-001",
                Description = "Wireless Bluetooth keyboard compatible with multiple devices",
                Price = 59.99m,
                Cost = 35.00m,
                CategoryId = electronicsCategory.Id,
                SupplierId = techSupplier.Id,
                LowStockThreshold = 15,
                Unit = "Piece",
                Barcode = "1234567890124"
            },
            new Product 
            { 
                Name = "USB-C Cable", 
                SKU = "ELEC-CB-001",
                Description = "6ft USB-C to USB-C cable for charging and data transfer",
                Price = 14.99m,
                Cost = 8.75m,
                CategoryId = electronicsCategory.Id,
                SupplierId = techSupplier.Id,
                LowStockThreshold = 50,
                Unit = "Piece",
                Barcode = "1234567890125"
            },
            
            // Clothing
            new Product 
            { 
                Name = "Cotton T-Shirt", 
                SKU = "CLTH-TS-001",
                Description = "Premium cotton crew neck t-shirt - Various sizes",
                Price = 19.99m,
                Cost = 12.00m,
                CategoryId = clothingCategory.Id,
                SupplierId = fashionSupplier.Id,
                LowStockThreshold = 25,
                Unit = "Piece",
                Barcode = "2234567890123"
            },
            
            // Books
            new Product 
            { 
                Name = "Business Management 101", 
                SKU = "BOOK-BM-001",
                Description = "Comprehensive guide to business management principles",
                Price = 34.99m,
                Cost = 20.00m,
                CategoryId = booksCategory.Id,
                SupplierId = bookSupplier.Id,
                LowStockThreshold = 10,
                Unit = "Piece",
                Barcode = "3234567890123"
            },
            
            // Office Supplies
            new Product 
            { 
                Name = "Black Ballpoint Pens", 
                SKU = "OFFC-BP-001",
                Description = "Pack of 12 black ballpoint pens",
                Price = 8.99m,
                Cost = 5.25m,
                CategoryId = officeCategory.Id,
                SupplierId = officeSupplier.Id,
                LowStockThreshold = 30,
                Unit = "Pack",
                Barcode = "4234567890123"
            }
        };

        context.Products.AddRange(products);
        logger.LogInformation("Added {Count} products.", products.Length);
    }

    /// <summary>
    /// Seed initial inventory
    /// </summary>
    private static async Task SeedInventoryAsync(AppDbContext context, ILogger logger)
    {
        if (await context.Inventories.AnyAsync())
        {
            logger.LogInformation("Inventory already exists, skipping seed.");
            return;
        }

        // Wait for products and warehouses to be saved
        await context.SaveChangesAsync();

        var products = await context.Products.ToListAsync();
        var warehouses = await context.Warehouses.Take(3).ToListAsync(); // First 3 warehouses
        var adminUser = await context.Users.FirstAsync(u => u.Username == "admin");

        var inventoryItems = new List<Inventory>();
        var transactions = new List<Transaction>();

        foreach (var product in products)
        {
            foreach (var warehouse in warehouses)
            {
                var quantity = warehouse.Name == "Main Warehouse" ? 100 : 
                              warehouse.Name == "North Distribution Center" ? 75 : 50;
                
                // Create inventory record
                var inventoryItem = new Inventory
                {
                    ProductId = product.Id,
                    WarehouseId = warehouse.Id,
                    Quantity = quantity,
                    ReservedQuantity = 0
                };
                
                inventoryItems.Add(inventoryItem);
                
                // Create initial stock-in transaction
                var transaction = Transaction.CreateStockIn(
                    productId: product.Id,
                    warehouseId: warehouse.Id,
                    userId: adminUser.Id,
                    quantity: quantity,
                    previousQuantity: 0,
                    unitCost: product.Cost,
                    reason: "Initial inventory setup",
                    referenceNumber: $"INV-SETUP-{product.SKU}-{warehouse.Name}"
                );
                
                transactions.Add(transaction);
            }
        }

        context.Inventories.AddRange(inventoryItems);
        context.Transactions.AddRange(transactions);
        
        logger.LogInformation("Added {InventoryCount} inventory items and {TransactionCount} transactions.", 
            inventoryItems.Count, transactions.Count);
    }
}