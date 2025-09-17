using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.WebUI.ViewModels.Dashboard;
using InventoryManagement.WebUI.ViewModels.Category;
using InventoryManagement.WebUI.ViewModels.Product;
using InventoryManagement.WebUI.ViewModels.Inventory;

namespace InventoryManagement.WebUI.Mappings;

/// <summary>
/// AutoMapper profile for mapping DTOs to ViewModels
/// </summary>
public class ViewModelMappingProfile : Profile
{
    public ViewModelMappingProfile()
    {
        // Dashboard mappings - DTO to ViewModel
        CreateMap<DashboardDataDto, DashboardViewModel>();
        CreateMap<ChartDataPointDto, ChartDataPoint>();
        CreateMap<RecentActivityDto, RecentActivityViewModel>();
        CreateMap<LowStockAlertDto, LowStockAlertViewModel>();
        CreateMap<TopProductDto, TopProductViewModel>();

        // Category mappings - DTO to ViewModel and ViewModel to DTO
        CreateMap<CategoryDto, CategoryViewModel>();
        CreateMap<CategoryDto, CategoryDetailsViewModel>();
        CreateMap<CategoryDto, EditCategoryViewModel>();
        CreateMap<CategoryDto, DeleteCategoryViewModel>();
        
        CreateMap<CreateCategoryViewModel, CreateCategoryDto>();
        CreateMap<EditCategoryViewModel, UpdateCategoryDto>();

        // Product mappings - DTO to ViewModel and ViewModel to DTO
        CreateMap<ProductDto, ProductDetailsViewModel>()
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.SKU ?? string.Empty))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Cost))
            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit ?? "Piece"))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.SupplierName ?? string.Empty))
            .ForMember(dest => dest.CurrentStock, opt => opt.MapFrom(src => src.TotalQuantity))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName ?? "Uncategorized"))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? string.Empty))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.LowStockThreshold, opt => opt.MapFrom(src => src.LowStockThreshold))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.Barcode, opt => opt.MapFrom(src => src.Barcode))
            .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
            .ForMember(dest => dest.Dimensions, opt => opt.MapFrom(src => src.Dimensions))
            .ForMember(dest => dest.RecentTransactions, opt => opt.Ignore())
            .ForMember(dest => dest.TotalStockIn30Days, opt => opt.Ignore())
            .ForMember(dest => dest.TotalStockOut30Days, opt => opt.Ignore())
            .ForMember(dest => dest.AverageMonthlyUsage, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Brand, opt => opt.Ignore());
        
        CreateMap<ProductDto, ProductEditViewModel>()
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.SKU ?? string.Empty))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Cost))
            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit ?? "Piece"))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.CurrentStock, opt => opt.MapFrom(src => src.TotalQuantity))
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.Suppliers, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Brand, opt => opt.Ignore())
            .ForMember(dest => dest.SupplierId, opt => opt.MapFrom(src => src.SupplierId));
        
        CreateMap<ProductDto, ProductItemViewModel>()
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.SKU));

        CreateMap<ProductDto, DeleteProductViewModel>()
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.SKU ?? string.Empty))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Cost))
            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit ?? "Piece"))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.CurrentStock, opt => opt.MapFrom(src => src.TotalQuantity))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName ?? "Uncategorized"))
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.SupplierName ?? string.Empty));
        
        CreateMap<ProductCreateViewModel, CreateProductDto>()
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.SKU));
        
        CreateMap<ProductEditViewModel, UpdateProductDto>()
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.SKU));
        
        CreateMap<InventoryManagement.Application.Extensions.PagedResult<ProductDto>, InventoryManagement.WebUI.ViewModels.Products.ProductIndexViewModel>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src));

        // Inventory mappings - DTO to ViewModel
        CreateMap<InventoryDto, InventoryDetailsViewModel>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
            .ForMember(dest => dest.ProductSku, opt => opt.MapFrom(src => src.ProductSKU))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location ?? "Not specified"))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.CategoryName, opt => opt.Ignore())
            .ForMember(dest => dest.TransactionTypeName, opt => opt.Ignore())
            .ForMember(dest => dest.Notes, opt => opt.Ignore())
            .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore());
    }
}
