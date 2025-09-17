using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Enums;
using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.Infrastructure.Persistence;

/// <summary>
/// Application database context for Entity Framework Core with Identity support
/// </summary>
public class AppDbContext : IdentityDbContext<IdentityUser>, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets for all entities
    public DbSet<Category> Categories { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public new DbSet<User> Users { get; set; } // Use 'new' to hide inherited Users property
    public DbSet<Product> Products { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all configurations
        ConfigureCategories(modelBuilder);
        ConfigureWarehouses(modelBuilder);
        ConfigureSuppliers(modelBuilder);
        ConfigureUsers(modelBuilder);
        ConfigureProducts(modelBuilder);
        ConfigureInventory(modelBuilder);
        ConfigureTransactions(modelBuilder);
        
        // Configure indexes for performance
        ConfigureIndexes(modelBuilder);
    }

    /// <summary>
    /// Configure Categories entity
    /// </summary>
    private static void ConfigureCategories(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Category>();
        
        entity.ToTable("Categories");
        
        entity.HasKey(e => e.Id);
        
        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();
            
        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        entity.Property(e => e.Description)
            .HasMaxLength(500);
            
        entity.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        entity.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        entity.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        // Unique constraint on Name
        entity.HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("UC_Categories_Name");
    }

    /// <summary>
    /// Configure Warehouses entity
    /// </summary>
    private static void ConfigureWarehouses(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Warehouse>();
        
        entity.ToTable("Warehouses");
        
        entity.HasKey(e => e.Id);
        
        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();
            
        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        entity.Property(e => e.Location)
            .IsRequired()
            .HasMaxLength(200);
            
        entity.Property(e => e.Address)
            .HasMaxLength(500);
            
        entity.Property(e => e.ContactPhone)
            .HasMaxLength(20);
            
        entity.Property(e => e.ContactEmail)
            .HasMaxLength(100);
            
        entity.Property(e => e.Capacity)
            .HasDefaultValue(null);
            
        entity.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        entity.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        entity.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        // Unique constraint on Name
        entity.HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("UC_Warehouses_Name");
    }

    /// <summary>
    /// Configure Suppliers entity
    /// </summary>
    private static void ConfigureSuppliers(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Supplier>();
        
        entity.ToTable("Suppliers");
        
        entity.HasKey(e => e.Id);
        
        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();
            
        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        entity.Property(e => e.ContactInfo)
            .HasMaxLength(200);
            
        entity.Property(e => e.Address)
            .HasMaxLength(500);
            
        entity.Property(e => e.Phone)
            .HasMaxLength(20);
            
        entity.Property(e => e.Email)
            .HasMaxLength(100);
            
        entity.Property(e => e.Website)
            .HasMaxLength(200);
            
        entity.Property(e => e.TaxNumber)
            .HasMaxLength(50);
            
        entity.Property(e => e.PaymentTerms)
            .HasMaxLength(100);
            
        entity.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        entity.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        entity.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        // Unique constraint on Name
        entity.HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("UC_Suppliers_Name");
    }

    /// <summary>
    /// Configure Users entity
    /// </summary>
    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<User>();
        
        entity.ToTable("Users");
        
        entity.HasKey(e => e.Id);
        
        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();
            
        entity.Property(e => e.Username)
            .IsRequired()
            .HasMaxLength(50);
            
        entity.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(100);
            
        entity.Property(e => e.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);
            
        entity.Property(e => e.FullName)
            .IsRequired()
            .HasMaxLength(100);
            
        entity.Property(e => e.Role)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (UserRole)Enum.Parse(typeof(UserRole), v))
            .HasMaxLength(20);
            
        entity.Property(e => e.PhoneNumber)
            .HasMaxLength(20);
            
        entity.Property(e => e.LastLoginAt)
            .HasDefaultValue(null);
            
        entity.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        entity.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        entity.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        // Unique constraints
        entity.HasIndex(e => e.Username)
            .IsUnique()
            .HasDatabaseName("UC_Users_Username");
            
        entity.HasIndex(e => e.Email)
            .IsUnique()
            .HasDatabaseName("UC_Users_Email");
    }

    /// <summary>
    /// Configure Products entity
    /// </summary>
    private static void ConfigureProducts(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Product>();
        
        entity.ToTable("Products");
        
        entity.HasKey(e => e.Id);
        
        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();
            
        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        entity.Property(e => e.SKU)
            .IsRequired()
            .HasMaxLength(50);
            
        entity.Property(e => e.Description)
            .HasMaxLength(1000);
            
        entity.Property(e => e.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
            
        entity.Property(e => e.Cost)
            .HasColumnType("decimal(18,2)");
            
        entity.Property(e => e.CategoryId)
            .IsRequired();
            
        entity.Property(e => e.SupplierId)
            .HasDefaultValue(null);
            
        entity.Property(e => e.LowStockThreshold)
            .IsRequired()
            .HasDefaultValue(10);
            
        entity.Property(e => e.Unit)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Piece");
            
        entity.Property(e => e.Barcode)
            .HasMaxLength(100);
            
        entity.Property(e => e.Weight)
            .HasColumnType("decimal(10,3)");
            
        entity.Property(e => e.Dimensions)
            .HasMaxLength(100);
            
        entity.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        entity.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        entity.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        // Unique constraint on SKU
        entity.HasIndex(e => e.SKU)
            .IsUnique()
            .HasDatabaseName("UC_Products_SKU");
            
        // Foreign key relationships
        entity.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .HasConstraintName("FK_Products_Category")
            .OnDelete(DeleteBehavior.Restrict);
            
        entity.HasOne(p => p.Supplier)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierId)
            .HasConstraintName("FK_Products_Supplier")
            .OnDelete(DeleteBehavior.SetNull);
    }

    /// <summary>
    /// Configure Inventory entity
    /// </summary>
    private static void ConfigureInventory(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Inventory>();
        
        entity.ToTable("Inventory");
        
        entity.HasKey(e => e.Id);
        
        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();
            
        entity.Property(e => e.ProductId)
            .IsRequired();
            
        entity.Property(e => e.WarehouseId)
            .IsRequired();
            
        entity.Property(e => e.Quantity)
            .IsRequired()
            .HasDefaultValue(0);
            
        entity.Property(e => e.ReservedQuantity)
            .IsRequired()
            .HasDefaultValue(0);
            
        entity.Property(e => e.LastStockCount)
            .HasDefaultValue(null);
            
        entity.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        entity.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        // Computed column for AvailableQuantity (handled in domain model as NotMapped)
        entity.Ignore(e => e.AvailableQuantity);
            
        // Unique constraint on ProductId + WarehouseId
        entity.HasIndex(e => new { e.ProductId, e.WarehouseId })
            .IsUnique()
            .HasDatabaseName("UC_Inventory_ProductWarehouse");
            
        // Foreign key relationships
        entity.HasOne(i => i.Product)
            .WithMany(p => p.InventoryItems)
            .HasForeignKey(i => i.ProductId)
            .HasConstraintName("FK_Inventory_Product")
            .OnDelete(DeleteBehavior.Cascade);
            
        entity.HasOne(i => i.Warehouse)
            .WithMany(w => w.InventoryItems)
            .HasForeignKey(i => i.WarehouseId)
            .HasConstraintName("FK_Inventory_Warehouse")
            .OnDelete(DeleteBehavior.Cascade);
    }

    /// <summary>
    /// Configure Transactions entity
    /// </summary>
    private static void ConfigureTransactions(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Transaction>();
        
        entity.ToTable("Transactions");
        
        entity.HasKey(e => e.Id);
        
        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();
            
        entity.Property(e => e.ProductId)
            .IsRequired();
            
        entity.Property(e => e.WarehouseId)
            .IsRequired();
            
        entity.Property(e => e.UserId)
            .IsRequired();
            
        entity.Property(e => e.TransactionType)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (TransactionType)Enum.Parse(typeof(TransactionType), v))
            .HasMaxLength(20);
            
        entity.Property(e => e.QuantityChanged)
            .IsRequired();
            
        entity.Property(e => e.PreviousQuantity)
            .IsRequired();
            
        entity.Property(e => e.NewQuantity)
            .IsRequired();
            
        entity.Property(e => e.UnitCost)
            .HasColumnType("decimal(18,2)");
            
        entity.Property(e => e.Reason)
            .HasMaxLength(200);
            
        entity.Property(e => e.ReferenceNumber)
            .HasMaxLength(50);
            
        entity.Property(e => e.Notes)
            .HasMaxLength(500);
            
        entity.Property(e => e.Timestamp)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        entity.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        // Computed column for TotalValue (handled in domain model as NotMapped)
        entity.Ignore(e => e.TotalValue);
            
        // Foreign key relationships
        entity.HasOne(t => t.Product)
            .WithMany(p => p.Transactions)
            .HasForeignKey(t => t.ProductId)
            .HasConstraintName("FK_Transactions_Product")
            .OnDelete(DeleteBehavior.Restrict);
            
        entity.HasOne(t => t.Warehouse)
            .WithMany(w => w.Transactions)
            .HasForeignKey(t => t.WarehouseId)
            .HasConstraintName("FK_Transactions_Warehouse")
            .OnDelete(DeleteBehavior.Restrict);
            
        entity.HasOne(t => t.User)
            .WithMany(u => u.Transactions)
            .HasForeignKey(t => t.UserId)
            .HasConstraintName("FK_Transactions_User")
            .OnDelete(DeleteBehavior.Restrict);
    }

    /// <summary>
    /// Configure database indexes for performance
    /// </summary>
    private static void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // Categories indexes
        modelBuilder.Entity<Category>()
            .HasIndex(e => e.IsActive)
            .HasDatabaseName("IX_Categories_IsActive");
            
        // Warehouses indexes
        modelBuilder.Entity<Warehouse>()
            .HasIndex(e => e.IsActive)
            .HasDatabaseName("IX_Warehouses_IsActive");
            
        // Suppliers indexes
        modelBuilder.Entity<Supplier>()
            .HasIndex(e => e.IsActive)
            .HasDatabaseName("IX_Suppliers_IsActive");
            
        // Users indexes
        modelBuilder.Entity<User>()
            .HasIndex(e => new { e.Role, e.IsActive })
            .HasDatabaseName("IX_Users_Role_IsActive");
            
        // Products indexes
        modelBuilder.Entity<Product>()
            .HasIndex(e => e.Name)
            .HasDatabaseName("IX_Products_Name");
            
        modelBuilder.Entity<Product>()
            .HasIndex(e => e.CategoryId)
            .HasDatabaseName("IX_Products_CategoryId");
            
        modelBuilder.Entity<Product>()
            .HasIndex(e => e.SupplierId)
            .HasDatabaseName("IX_Products_SupplierId");
            
        modelBuilder.Entity<Product>()
            .HasIndex(e => e.IsActive)
            .HasDatabaseName("IX_Products_IsActive");
            
        modelBuilder.Entity<Product>()
            .HasIndex(e => e.Barcode)
            .HasDatabaseName("IX_Products_Barcode");
            
        // Inventory indexes
        modelBuilder.Entity<Inventory>()
            .HasIndex(e => e.ProductId)
            .HasDatabaseName("IX_Inventory_ProductId");
            
        modelBuilder.Entity<Inventory>()
            .HasIndex(e => e.WarehouseId)
            .HasDatabaseName("IX_Inventory_WarehouseId");
            
        modelBuilder.Entity<Inventory>()
            .HasIndex(e => e.Quantity)
            .HasDatabaseName("IX_Inventory_LowStock");
            
        // Transactions indexes
        modelBuilder.Entity<Transaction>()
            .HasIndex(e => new { e.ProductId, e.Timestamp })
            .HasDatabaseName("IX_Transactions_ProductId_Timestamp");
            
        modelBuilder.Entity<Transaction>()
            .HasIndex(e => new { e.WarehouseId, e.Timestamp })
            .HasDatabaseName("IX_Transactions_WarehouseId_Timestamp");
            
        modelBuilder.Entity<Transaction>()
            .HasIndex(e => new { e.UserId, e.Timestamp })
            .HasDatabaseName("IX_Transactions_UserId_Timestamp");
            
        modelBuilder.Entity<Transaction>()
            .HasIndex(e => new { e.TransactionType, e.Timestamp })
            .HasDatabaseName("IX_Transactions_TransactionType_Timestamp");
            
        modelBuilder.Entity<Transaction>()
            .HasIndex(e => e.Timestamp)
            .HasDatabaseName("IX_Transactions_Timestamp");
            
        modelBuilder.Entity<Transaction>()
            .HasIndex(e => e.ReferenceNumber)
            .HasDatabaseName("IX_Transactions_ReferenceNumber");
    }

    /// <summary>
    /// Override SaveChanges to update timestamps automatically
    /// </summary>
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override SaveChangesAsync to update timestamps automatically
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Automatically update timestamps for entities
    /// </summary>
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                    
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
    }
}