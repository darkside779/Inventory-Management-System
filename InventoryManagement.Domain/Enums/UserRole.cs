namespace InventoryManagement.Domain.Enums;

/// <summary>
/// Represents the different user roles in the inventory management system
/// </summary>
public enum UserRole
{
    /// <summary>
    /// System administrator with full access to all features
    /// </summary>
    Administrator = 1,
    
    /// <summary>
    /// Manager with access to inventory management and reporting
    /// </summary>
    Manager = 2,
    
    /// <summary>
    /// Staff with limited access to transaction processing
    /// </summary>
    Staff = 3
}