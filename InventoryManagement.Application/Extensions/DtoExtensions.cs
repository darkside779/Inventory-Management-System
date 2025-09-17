using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Extensions;

/// <summary>
/// Extension methods for convenient DTO mapping operations
/// </summary>
public static class DtoExtensions
{
    /// <summary>
    /// Maps a Category entity to CategoryDto
    /// </summary>
    public static CategoryDto ToDto(this Category category, IMapper mapper)
    {
        return mapper.Map<CategoryDto>(category);
    }

    /// <summary>
    /// Maps a collection of Category entities to CategoryDto collection
    /// </summary>
    public static IEnumerable<CategoryDto> ToDto(this IEnumerable<Category> categories, IMapper mapper)
    {
        return mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    /// <summary>
    /// Maps a Category entity to CategoryLookupDto
    /// </summary>
    public static CategoryLookupDto ToLookupDto(this Category category, IMapper mapper)
    {
        return mapper.Map<CategoryLookupDto>(category);
    }

    /// <summary>
    /// Maps a Warehouse entity to WarehouseDto
    /// </summary>
    public static WarehouseDto ToDto(this Warehouse warehouse, IMapper mapper)
    {
        return mapper.Map<WarehouseDto>(warehouse);
    }

    /// <summary>
    /// Maps a collection of Warehouse entities to WarehouseDto collection
    /// </summary>
    public static IEnumerable<WarehouseDto> ToDto(this IEnumerable<Warehouse> warehouses, IMapper mapper)
    {
        return mapper.Map<IEnumerable<WarehouseDto>>(warehouses);
    }

    /// <summary>
    /// Maps a Warehouse entity to WarehouseLookupDto
    /// </summary>
    public static WarehouseLookupDto ToLookupDto(this Warehouse warehouse, IMapper mapper)
    {
        return mapper.Map<WarehouseLookupDto>(warehouse);
    }

    /// <summary>
    /// Maps a Supplier entity to SupplierDto
    /// </summary>
    public static SupplierDto ToDto(this Supplier supplier, IMapper mapper)
    {
        return mapper.Map<SupplierDto>(supplier);
    }

    /// <summary>
    /// Maps a collection of Supplier entities to SupplierDto collection
    /// </summary>
    public static IEnumerable<SupplierDto> ToDto(this IEnumerable<Supplier> suppliers, IMapper mapper)
    {
        return mapper.Map<IEnumerable<SupplierDto>>(suppliers);
    }

    /// <summary>
    /// Maps a Supplier entity to SupplierLookupDto
    /// </summary>
    public static SupplierLookupDto ToLookupDto(this Supplier supplier, IMapper mapper)
    {
        return mapper.Map<SupplierLookupDto>(supplier);
    }

    /// <summary>
    /// Maps a User entity to UserDto
    /// </summary>
    public static UserDto ToDto(this User user, IMapper mapper)
    {
        return mapper.Map<UserDto>(user);
    }

    /// <summary>
    /// Maps a collection of User entities to UserDto collection
    /// </summary>
    public static IEnumerable<UserDto> ToDto(this IEnumerable<User> users, IMapper mapper)
    {
        return mapper.Map<IEnumerable<UserDto>>(users);
    }

    /// <summary>
    /// Maps a User entity to UserLookupDto
    /// </summary>
    public static UserLookupDto ToLookupDto(this User user, IMapper mapper)
    {
        return mapper.Map<UserLookupDto>(user);
    }

    /// <summary>
    /// Maps a Product entity to ProductDto
    /// </summary>
    public static ProductDto ToDto(this Product product, IMapper mapper)
    {
        return mapper.Map<ProductDto>(product);
    }

    /// <summary>
    /// Maps a collection of Product entities to ProductDto collection
    /// </summary>
    public static IEnumerable<ProductDto> ToDto(this IEnumerable<Product> products, IMapper mapper)
    {
        return mapper.Map<IEnumerable<ProductDto>>(products);
    }

    /// <summary>
    /// Maps a Product entity to ProductLookupDto
    /// </summary>
    public static ProductLookupDto ToLookupDto(this Product product, IMapper mapper)
    {
        return mapper.Map<ProductLookupDto>(product);
    }

    /// <summary>
    /// Maps an Inventory entity to InventoryDto
    /// </summary>
    public static InventoryDto ToDto(this Inventory inventory, IMapper mapper)
    {
        return mapper.Map<InventoryDto>(inventory);
    }

    /// <summary>
    /// Maps a collection of Inventory entities to InventoryDto collection
    /// </summary>
    public static IEnumerable<InventoryDto> ToDto(this IEnumerable<Inventory> inventories, IMapper mapper)
    {
        return mapper.Map<IEnumerable<InventoryDto>>(inventories);
    }

    /// <summary>
    /// Maps an Inventory entity to InventoryLookupDto
    /// </summary>
    public static InventoryLookupDto ToLookupDto(this Inventory inventory, IMapper mapper)
    {
        return mapper.Map<InventoryLookupDto>(inventory);
    }

    /// <summary>
    /// Maps a Transaction entity to TransactionDto
    /// </summary>
    public static TransactionDto ToDto(this Transaction transaction, IMapper mapper)
    {
        return mapper.Map<TransactionDto>(transaction);
    }

    /// <summary>
    /// Maps a collection of Transaction entities to TransactionDto collection
    /// </summary>
    public static IEnumerable<TransactionDto> ToDto(this IEnumerable<Transaction> transactions, IMapper mapper)
    {
        return mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    /// <summary>
    /// Maps a Transaction entity to TransactionLookupDto
    /// </summary>
    public static TransactionLookupDto ToLookupDto(this Transaction transaction, IMapper mapper)
    {
        return mapper.Map<TransactionLookupDto>(transaction);
    }

    /// <summary>
    /// Maps CreateCategoryDto to Category entity
    /// </summary>
    public static Category ToEntity(this CreateCategoryDto createDto, IMapper mapper)
    {
        return mapper.Map<Category>(createDto);
    }

    /// <summary>
    /// Maps UpdateCategoryDto to Category entity
    /// </summary>
    public static Category ToEntity(this UpdateCategoryDto updateDto, IMapper mapper)
    {
        return mapper.Map<Category>(updateDto);
    }

    /// <summary>
    /// Maps CreateWarehouseDto to Warehouse entity
    /// </summary>
    public static Warehouse ToEntity(this CreateWarehouseDto createDto, IMapper mapper)
    {
        return mapper.Map<Warehouse>(createDto);
    }

    /// <summary>
    /// Maps UpdateWarehouseDto to Warehouse entity
    /// </summary>
    public static Warehouse ToEntity(this UpdateWarehouseDto updateDto, IMapper mapper)
    {
        return mapper.Map<Warehouse>(updateDto);
    }

    /// <summary>
    /// Maps CreateSupplierDto to Supplier entity
    /// </summary>
    public static Supplier ToEntity(this CreateSupplierDto createDto, IMapper mapper)
    {
        return mapper.Map<Supplier>(createDto);
    }

    /// <summary>
    /// Maps UpdateSupplierDto to Supplier entity
    /// </summary>
    public static Supplier ToEntity(this UpdateSupplierDto updateDto, IMapper mapper)
    {
        return mapper.Map<Supplier>(updateDto);
    }

    /// <summary>
    /// Maps CreateUserDto to User entity
    /// </summary>
    public static User ToEntity(this CreateUserDto createDto, IMapper mapper)
    {
        return mapper.Map<User>(createDto);
    }

    /// <summary>
    /// Maps UpdateUserDto to User entity
    /// </summary>
    public static User ToEntity(this UpdateUserDto updateDto, IMapper mapper)
    {
        return mapper.Map<User>(updateDto);
    }

    /// <summary>
    /// Maps CreateProductDto to Product entity
    /// </summary>
    public static Product ToEntity(this CreateProductDto createDto, IMapper mapper)
    {
        return mapper.Map<Product>(createDto);
    }

    /// <summary>
    /// Maps UpdateProductDto to Product entity
    /// </summary>
    public static Product ToEntity(this UpdateProductDto updateDto, IMapper mapper)
    {
        return mapper.Map<Product>(updateDto);
    }

    /// <summary>
    /// Maps CreateInventoryDto to Inventory entity
    /// </summary>
    public static Inventory ToEntity(this CreateInventoryDto createDto, IMapper mapper)
    {
        return mapper.Map<Inventory>(createDto);
    }

    /// <summary>
    /// Maps UpdateInventoryDto to Inventory entity
    /// </summary>
    public static Inventory ToEntity(this UpdateInventoryDto updateDto, IMapper mapper)
    {
        return mapper.Map<Inventory>(updateDto);
    }

    /// <summary>
    /// Maps CreateTransactionDto to Transaction entity
    /// </summary>
    public static Transaction ToEntity(this CreateTransactionDto createDto, IMapper mapper)
    {
        return mapper.Map<Transaction>(createDto);
    }

    /// <summary>
    /// Maps UpdateTransactionDto to Transaction entity
    /// </summary>
    public static Transaction ToEntity(this UpdateTransactionDto updateDto, IMapper mapper)
    {
        return mapper.Map<Transaction>(updateDto);
    }
}

/// <summary>
/// Extension methods for paginated results
/// </summary>
public static class PaginationExtensions
{
    /// <summary>
    /// Maps a paginated result of entities to DTOs
    /// </summary>
    public static PagedResult<TDto> ToPagedDto<TEntity, TDto>(
        this (IEnumerable<TEntity> Items, int TotalCount) pagedResult,
        IMapper mapper) where TEntity : class where TDto : class
    {
        return new PagedResult<TDto>
        {
            Items = mapper.Map<IEnumerable<TDto>>(pagedResult.Items),
            TotalCount = pagedResult.TotalCount
        };
    }
}

/// <summary>
/// Represents a paginated result
/// </summary>
public class PagedResult<T>
{
    /// <summary>
    /// Items in the current page
    /// </summary>
    public IEnumerable<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Total count of items across all pages
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
}
