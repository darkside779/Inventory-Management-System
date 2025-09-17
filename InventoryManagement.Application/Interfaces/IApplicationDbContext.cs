using Microsoft.EntityFrameworkCore;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces;

/// <summary>
/// Interface for the application database context
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Category> Categories { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<Inventory> Inventories { get; set; }
    DbSet<Warehouse> Warehouses { get; set; }
    DbSet<Supplier> Suppliers { get; set; }
    DbSet<Transaction> Transactions { get; set; }
    DbSet<User> Users { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
}
