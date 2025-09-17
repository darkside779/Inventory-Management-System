using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Mappings;

/// <summary>
/// AutoMapper mapping profile for entity-to-DTO mappings
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Category mappings
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();
        CreateMap<Category, CategoryLookupDto>();

        // Warehouse mappings
        CreateMap<Warehouse, WarehouseDto>()
            .ForMember(dest => dest.InventoryItemCount, opt => opt.MapFrom(src => src.InventoryItems.Count))
            .ForMember(dest => dest.CapacityUtilization, opt => opt.MapFrom(src => 
                src.Capacity.HasValue && src.Capacity > 0 
                    ? (decimal)src.InventoryItems.Sum(i => i.Quantity) / src.Capacity.Value * 100 
                    : (decimal?)null));
        CreateMap<CreateWarehouseDto, Warehouse>();
        CreateMap<UpdateWarehouseDto, Warehouse>();
        CreateMap<Warehouse, WarehouseLookupDto>();

        // Supplier mappings
        CreateMap<Supplier, SupplierDto>()
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));
        CreateMap<CreateSupplierDto, Supplier>();
        CreateMap<UpdateSupplierDto, Supplier>();
        CreateMap<Supplier, SupplierLookupDto>();

        // User mappings
        CreateMap<User, UserDto>()
            .AfterMap((src, dest) =>
            {
                var nameParts = src.FullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                dest.FirstName = nameParts.Length > 0 ? nameParts[0] : "";
                dest.LastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";
            });
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password)) // Note: Password should be hashed in service layer
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Don't map password in update
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
        CreateMap<User, UserLookupDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));

        // Product mappings
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : null))
            .ForMember(dest => dest.TotalQuantity, opt => opt.MapFrom(src => src.InventoryItems.Sum(i => i.Quantity)))
            .ForMember(dest => dest.ProfitMarginPercentage, opt => opt.MapFrom(src => 
                src.Cost.HasValue && src.Cost > 0 
                    ? ((src.Price - src.Cost.Value) / src.Cost.Value) * 100 
                    : (decimal?)null));
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Supplier, opt => opt.Ignore());
        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Supplier, opt => opt.Ignore());
        CreateMap<Product, ProductLookupDto>();

        // Dashboard DTO mappings (ViewModel mappings are handled in WebUI layer)
        CreateMap<ChartDataPointDto, ChartDataPointDto>();
        CreateMap<RecentActivityDto, RecentActivityDto>();
        CreateMap<LowStockAlertDto, LowStockAlertDto>();
        CreateMap<TopProductDto, TopProductDto>();

        // Inventory mappings
        CreateMap<Inventory, InventoryDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductSKU, opt => opt.MapFrom(src => src.Product.SKU))
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse.Name))
            .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
            .ForMember(dest => dest.MinimumStockLevel, opt => opt.MapFrom(src => 0)) // Default value since not in entity
            .ForMember(dest => dest.MaximumStockLevel, opt => opt.MapFrom(src => 1000)); // Default value since not in entity
        CreateMap<CreateInventoryDto, Inventory>();
        CreateMap<UpdateInventoryDto, Inventory>();
        CreateMap<Inventory, InventoryLookupDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductSKU, opt => opt.MapFrom(src => src.Product.SKU))
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse.Name))
            .ForMember(dest => dest.AvailableQuantity, opt => opt.MapFrom(src => src.Quantity - src.ReservedQuantity))
            .ForMember(dest => dest.IsBelowMinimum, opt => opt.MapFrom(src => src.Product != null && src.Quantity <= src.Product.LowStockThreshold));

        // Transaction mappings
        CreateMap<Transaction, TransactionDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
            .ForMember(dest => dest.ProductSKU, opt => opt.MapFrom(src => src.Product != null ? src.Product.SKU : string.Empty))
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : string.Empty))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TransactionType))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.QuantityChanged))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitCost))
            .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.Timestamp))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes ?? string.Empty));
        CreateMap<CreateTransactionDto, Transaction>()
            .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.QuantityChanged, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.UnitCost, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.TransactionDate))
            .ForMember(dest => dest.ReferenceNumber, opt => opt.MapFrom(src => src.ReferenceNumber))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));
        CreateMap<UpdateTransactionDto, Transaction>()
            .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.QuantityChanged, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.UnitCost, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.TransactionDate))
            .ForMember(dest => dest.ReferenceNumber, opt => opt.MapFrom(src => src.ReferenceNumber))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));
        CreateMap<Transaction, TransactionLookupDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : string.Empty))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TransactionType))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.QuantityChanged))
            .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.Timestamp));


        // Reverse mappings for updates
        CreateMap<CategoryDto, Category>();
        CreateMap<WarehouseDto, Warehouse>();
        CreateMap<SupplierDto, Supplier>();
        CreateMap<UserDto, User>();
        CreateMap<ProductDto, Product>();
        CreateMap<InventoryDto, Inventory>();
        CreateMap<TransactionDto, Transaction>();
    }
}
