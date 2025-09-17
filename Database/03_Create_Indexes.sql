-- =====================================================
-- Inventory Management System - Performance Indexes
-- SQL Server 14 Compatible
-- =====================================================

USE [InventoryManagementDB];
GO

PRINT 'Creating performance indexes...';

-- =====================================================
-- Categories Table Indexes
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Categories_Name' AND object_id = OBJECT_ID('Categories'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Categories_Name] 
    ON [dbo].[Categories] ([Name] ASC)
    INCLUDE ([IsActive]);
    PRINT 'Index IX_Categories_Name created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Categories_IsActive' AND object_id = OBJECT_ID('Categories'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Categories_IsActive] 
    ON [dbo].[Categories] ([IsActive] ASC)
    INCLUDE ([Name], [CreatedAt]);
    PRINT 'Index IX_Categories_IsActive created.';
END

-- =====================================================
-- Warehouses Table Indexes
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Warehouses_Name' AND object_id = OBJECT_ID('Warehouses'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Warehouses_Name] 
    ON [dbo].[Warehouses] ([Name] ASC)
    INCLUDE ([Location], [IsActive]);
    PRINT 'Index IX_Warehouses_Name created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Warehouses_IsActive' AND object_id = OBJECT_ID('Warehouses'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Warehouses_IsActive] 
    ON [dbo].[Warehouses] ([IsActive] ASC)
    INCLUDE ([Name], [Location]);
    PRINT 'Index IX_Warehouses_IsActive created.';
END

-- =====================================================
-- Suppliers Table Indexes
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Suppliers_Name' AND object_id = OBJECT_ID('Suppliers'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Suppliers_Name] 
    ON [dbo].[Suppliers] ([Name] ASC)
    INCLUDE ([ContactInfo], [IsActive]);
    PRINT 'Index IX_Suppliers_Name created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Suppliers_IsActive' AND object_id = OBJECT_ID('Suppliers'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Suppliers_IsActive] 
    ON [dbo].[Suppliers] ([IsActive] ASC)
    INCLUDE ([Name], [ContactInfo]);
    PRINT 'Index IX_Suppliers_IsActive created.';
END

-- =====================================================
-- Users Table Indexes
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Username' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Users_Username] 
    ON [dbo].[Users] ([Username] ASC)
    INCLUDE ([Email], [Role], [IsActive]);
    PRINT 'Index IX_Users_Username created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Users_Email] 
    ON [dbo].[Users] ([Email] ASC)
    INCLUDE ([Username], [Role], [IsActive]);
    PRINT 'Index IX_Users_Email created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Role_IsActive' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Users_Role_IsActive] 
    ON [dbo].[Users] ([Role] ASC, [IsActive] ASC)
    INCLUDE ([Username], [FullName], [Email]);
    PRINT 'Index IX_Users_Role_IsActive created.';
END

-- =====================================================
-- Products Table Indexes
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_SKU' AND object_id = OBJECT_ID('Products'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Products_SKU] 
    ON [dbo].[Products] ([SKU] ASC)
    INCLUDE ([Name], [Price], [IsActive]);
    PRINT 'Index IX_Products_SKU created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_Name' AND object_id = OBJECT_ID('Products'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Products_Name] 
    ON [dbo].[Products] ([Name] ASC)
    INCLUDE ([SKU], [Price], [CategoryId], [IsActive]);
    PRINT 'Index IX_Products_Name created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_CategoryId' AND object_id = OBJECT_ID('Products'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Products_CategoryId] 
    ON [dbo].[Products] ([CategoryId] ASC)
    INCLUDE ([Name], [SKU], [Price], [IsActive]);
    PRINT 'Index IX_Products_CategoryId created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_SupplierId' AND object_id = OBJECT_ID('Products'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Products_SupplierId] 
    ON [dbo].[Products] ([SupplierId] ASC)
    INCLUDE ([Name], [SKU], [Price], [IsActive]);
    PRINT 'Index IX_Products_SupplierId created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_IsActive' AND object_id = OBJECT_ID('Products'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Products_IsActive] 
    ON [dbo].[Products] ([IsActive] ASC)
    INCLUDE ([Name], [SKU], [CategoryId], [Price]);
    PRINT 'Index IX_Products_IsActive created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_Barcode' AND object_id = OBJECT_ID('Products'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Products_Barcode] 
    ON [dbo].[Products] ([Barcode] ASC)
    INCLUDE ([Name], [SKU], [Price])
    WHERE [Barcode] IS NOT NULL;
    PRINT 'Index IX_Products_Barcode created.';
END

-- =====================================================
-- Inventory Table Indexes (Critical for Performance)
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Inventory_ProductId' AND object_id = OBJECT_ID('Inventory'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Inventory_ProductId] 
    ON [dbo].[Inventory] ([ProductId] ASC)
    INCLUDE ([WarehouseId], [Quantity], [AvailableQuantity]);
    PRINT 'Index IX_Inventory_ProductId created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Inventory_WarehouseId' AND object_id = OBJECT_ID('Inventory'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Inventory_WarehouseId] 
    ON [dbo].[Inventory] ([WarehouseId] ASC)
    INCLUDE ([ProductId], [Quantity], [AvailableQuantity]);
    PRINT 'Index IX_Inventory_WarehouseId created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Inventory_LowStock' AND object_id = OBJECT_ID('Inventory'))
BEGIN
    -- This index will help identify low stock items quickly
    CREATE NONCLUSTERED INDEX [IX_Inventory_LowStock] 
    ON [dbo].[Inventory] ([Quantity] ASC)
    INCLUDE ([ProductId], [WarehouseId], [UpdatedAt]);
    PRINT 'Index IX_Inventory_LowStock created.';
END

-- =====================================================
-- Transactions Table Indexes (Heavy Read/Write Table)
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Transactions_ProductId_Timestamp' AND object_id = OBJECT_ID('Transactions'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Transactions_ProductId_Timestamp] 
    ON [dbo].[Transactions] ([ProductId] ASC, [Timestamp] DESC)
    INCLUDE ([TransactionType], [QuantityChanged], [UserId], [WarehouseId]);
    PRINT 'Index IX_Transactions_ProductId_Timestamp created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Transactions_WarehouseId_Timestamp' AND object_id = OBJECT_ID('Transactions'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Transactions_WarehouseId_Timestamp] 
    ON [dbo].[Transactions] ([WarehouseId] ASC, [Timestamp] DESC)
    INCLUDE ([ProductId], [TransactionType], [QuantityChanged], [UserId]);
    PRINT 'Index IX_Transactions_WarehouseId_Timestamp created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Transactions_UserId_Timestamp' AND object_id = OBJECT_ID('Transactions'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Transactions_UserId_Timestamp] 
    ON [dbo].[Transactions] ([UserId] ASC, [Timestamp] DESC)
    INCLUDE ([ProductId], [WarehouseId], [TransactionType], [QuantityChanged]);
    PRINT 'Index IX_Transactions_UserId_Timestamp created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Transactions_TransactionType_Timestamp' AND object_id = OBJECT_ID('Transactions'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Transactions_TransactionType_Timestamp] 
    ON [dbo].[Transactions] ([TransactionType] ASC, [Timestamp] DESC)
    INCLUDE ([ProductId], [WarehouseId], [QuantityChanged], [UserId]);
    PRINT 'Index IX_Transactions_TransactionType_Timestamp created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Transactions_Timestamp' AND object_id = OBJECT_ID('Transactions'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Transactions_Timestamp] 
    ON [dbo].[Transactions] ([Timestamp] DESC)
    INCLUDE ([ProductId], [WarehouseId], [TransactionType], [QuantityChanged], [UserId]);
    PRINT 'Index IX_Transactions_Timestamp created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Transactions_ReferenceNumber' AND object_id = OBJECT_ID('Transactions'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Transactions_ReferenceNumber] 
    ON [dbo].[Transactions] ([ReferenceNumber] ASC)
    INCLUDE ([ProductId], [TransactionType], [Timestamp])
    WHERE [ReferenceNumber] IS NOT NULL;
    PRINT 'Index IX_Transactions_ReferenceNumber created.';
END

-- =====================================================
-- Composite Indexes for Common Query Patterns
-- =====================================================

-- Products with low stock threshold check
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_CategoryId_IsActive_LowStock' AND object_id = OBJECT_ID('Products'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Products_CategoryId_IsActive_LowStock] 
    ON [dbo].[Products] ([CategoryId] ASC, [IsActive] ASC, [LowStockThreshold] ASC)
    INCLUDE ([Name], [SKU], [Price]);
    PRINT 'Index IX_Products_CategoryId_IsActive_LowStock created.';
END

-- Date range queries for reporting
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Transactions_DateRange_Reporting' AND object_id = OBJECT_ID('Transactions'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Transactions_DateRange_Reporting] 
    ON [dbo].[Transactions] ([Timestamp] ASC, [TransactionType] ASC)
    INCLUDE ([ProductId], [WarehouseId], [QuantityChanged], [TotalValue]);
    PRINT 'Index IX_Transactions_DateRange_Reporting created.';
END

PRINT 'All performance indexes created successfully!';

-- =====================================================
-- Create Statistics for Query Optimization
-- =====================================================
PRINT 'Updating statistics...';

UPDATE STATISTICS [dbo].[Categories];
UPDATE STATISTICS [dbo].[Warehouses];
UPDATE STATISTICS [dbo].[Suppliers];
UPDATE STATISTICS [dbo].[Users];
UPDATE STATISTICS [dbo].[Products];
UPDATE STATISTICS [dbo].[Inventory];
UPDATE STATISTICS [dbo].[Transactions];

PRINT 'Statistics updated successfully!';
PRINT 'Database indexing completed - Performance optimized!';