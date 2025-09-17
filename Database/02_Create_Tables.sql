-- =====================================================
-- Inventory Management System - Table Creation
-- SQL Server 14 Compatible
-- =====================================================

USE [InventoryManagementDB];
GO

-- =====================================================
-- 1. Categories Table (Referenced by Products)
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Categories' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Categories] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsActive] BIT NOT NULL DEFAULT 1,
        
        CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UC_Categories_Name] UNIQUE ([Name])
    );
    
    PRINT 'Categories table created successfully.';
END
ELSE
    PRINT 'Categories table already exists.';
GO

-- =====================================================
-- 2. Warehouses Table (Referenced by Inventory & Transactions)
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Warehouses' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Warehouses] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Location] NVARCHAR(200) NOT NULL,
        [Address] NVARCHAR(500) NULL,
        [ContactPhone] NVARCHAR(20) NULL,
        [ContactEmail] NVARCHAR(100) NULL,
        [Capacity] INT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsActive] BIT NOT NULL DEFAULT 1,
        
        CONSTRAINT [PK_Warehouses] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UC_Warehouses_Name] UNIQUE ([Name])
    );
    
    PRINT 'Warehouses table created successfully.';
END
ELSE
    PRINT 'Warehouses table already exists.';
GO

-- =====================================================
-- 3. Suppliers Table (Referenced by Products)
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Suppliers' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Suppliers] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [ContactInfo] NVARCHAR(200) NULL,
        [Address] NVARCHAR(500) NULL,
        [Phone] NVARCHAR(20) NULL,
        [Email] NVARCHAR(100) NULL,
        [Website] NVARCHAR(200) NULL,
        [TaxNumber] NVARCHAR(50) NULL,
        [PaymentTerms] NVARCHAR(100) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsActive] BIT NOT NULL DEFAULT 1,
        
        CONSTRAINT [PK_Suppliers] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UC_Suppliers_Name] UNIQUE ([Name])
    );
    
    PRINT 'Suppliers table created successfully.';
END
ELSE
    PRINT 'Suppliers table already exists.';
GO

-- =====================================================
-- 4. Users Table (For Authentication & Auditing)
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Username] NVARCHAR(50) NOT NULL,
        [Email] NVARCHAR(100) NOT NULL,
        [PasswordHash] NVARCHAR(256) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [Role] NVARCHAR(20) NOT NULL, -- Administrator, Manager, Staff
        [PhoneNumber] NVARCHAR(20) NULL,
        [LastLoginAt] DATETIME2 NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsActive] BIT NOT NULL DEFAULT 1,
        
        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UC_Users_Username] UNIQUE ([Username]),
        CONSTRAINT [UC_Users_Email] UNIQUE ([Email]),
        CONSTRAINT [CK_Users_Role] CHECK ([Role] IN ('Administrator', 'Manager', 'Staff'))
    );
    
    PRINT 'Users table created successfully.';
END
ELSE
    PRINT 'Users table already exists.';
GO

-- =====================================================
-- 5. Products Table (Main Product Catalog)
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Products' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Products] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [SKU] NVARCHAR(50) NOT NULL,
        [Description] NVARCHAR(1000) NULL,
        [Price] DECIMAL(18,2) NOT NULL,
        [Cost] DECIMAL(18,2) NULL,
        [CategoryId] INT NOT NULL,
        [SupplierId] INT NULL,
        [LowStockThreshold] INT NOT NULL DEFAULT 10,
        [Unit] NVARCHAR(20) NOT NULL DEFAULT 'Piece',
        [Barcode] NVARCHAR(100) NULL,
        [Weight] DECIMAL(10,3) NULL,
        [Dimensions] NVARCHAR(100) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsActive] BIT NOT NULL DEFAULT 1,
        
        CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UC_Products_SKU] UNIQUE ([SKU]),
        CONSTRAINT [FK_Products_Category] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories]([Id]),
        CONSTRAINT [FK_Products_Supplier] FOREIGN KEY ([SupplierId]) REFERENCES [dbo].[Suppliers]([Id]),
        CONSTRAINT [CK_Products_Price] CHECK ([Price] >= 0),
        CONSTRAINT [CK_Products_Cost] CHECK ([Cost] >= 0),
        CONSTRAINT [CK_Products_LowStockThreshold] CHECK ([LowStockThreshold] >= 0)
    );
    
    PRINT 'Products table created successfully.';
END
ELSE
    PRINT 'Products table already exists.';
GO

-- =====================================================
-- 6. Inventory Table (Stock Levels per Warehouse)
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Inventory' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Inventory] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [ProductId] INT NOT NULL,
        [WarehouseId] INT NOT NULL,
        [Quantity] INT NOT NULL DEFAULT 0,
        [ReservedQuantity] INT NOT NULL DEFAULT 0,
        [AvailableQuantity] AS ([Quantity] - [ReservedQuantity]) PERSISTED,
        [LastStockCount] DATETIME2 NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT [PK_Inventory] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UC_Inventory_ProductWarehouse] UNIQUE ([ProductId], [WarehouseId]),
        CONSTRAINT [FK_Inventory_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products]([Id]),
        CONSTRAINT [FK_Inventory_Warehouse] FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[Warehouses]([Id]),
        CONSTRAINT [CK_Inventory_Quantity] CHECK ([Quantity] >= 0),
        CONSTRAINT [CK_Inventory_Reserved] CHECK ([ReservedQuantity] >= 0 AND [ReservedQuantity] <= [Quantity])
    );
    
    PRINT 'Inventory table created successfully.';
END
ELSE
    PRINT 'Inventory table already exists.';
GO

-- =====================================================
-- 7. Transactions Table (All Stock Movements)
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Transactions' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Transactions] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [ProductId] INT NOT NULL,
        [WarehouseId] INT NOT NULL,
        [UserId] INT NOT NULL,
        [TransactionType] NVARCHAR(20) NOT NULL, -- StockIn, StockOut, Adjustment
        [QuantityChanged] INT NOT NULL,
        [PreviousQuantity] INT NOT NULL,
        [NewQuantity] INT NOT NULL,
        [UnitCost] DECIMAL(18,2) NULL,
        [TotalValue] AS ([QuantityChanged] * [UnitCost]) PERSISTED,
        [Reason] NVARCHAR(200) NULL,
        [ReferenceNumber] NVARCHAR(50) NULL,
        [Notes] NVARCHAR(500) NULL,
        [Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Transactions_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products]([Id]),
        CONSTRAINT [FK_Transactions_Warehouse] FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[Warehouses]([Id]),
        CONSTRAINT [FK_Transactions_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]),
        CONSTRAINT [CK_Transactions_Type] CHECK ([TransactionType] IN ('StockIn', 'StockOut', 'Adjustment')),
        CONSTRAINT [CK_Transactions_Quantity] CHECK ([QuantityChanged] != 0),
        CONSTRAINT [CK_Transactions_PreviousQuantity] CHECK ([PreviousQuantity] >= 0),
        CONSTRAINT [CK_Transactions_NewQuantity] CHECK ([NewQuantity] >= 0),
        CONSTRAINT [CK_Transactions_UnitCost] CHECK ([UnitCost] >= 0)
    );
    
    PRINT 'Transactions table created successfully.';
END
ELSE
    PRINT 'Transactions table already exists.';
GO

PRINT 'All tables created successfully!';