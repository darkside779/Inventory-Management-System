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

-- =====================================================
-- 8. Customers Table
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Customers' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Customers] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [CustomerCode] NVARCHAR(20) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [CompanyName] NVARCHAR(100) NULL,
        [Email] NVARCHAR(100) NULL,
        [PhoneNumber] NVARCHAR(20) NULL,
        [Address] NVARCHAR(500) NULL,
        [CustomerType] NVARCHAR(20) NOT NULL,
        [Balance] DECIMAL(18,2) NOT NULL,
        [CreditLimit] DECIMAL(18,2) NOT NULL,
        [PaymentTermsDays] INT NOT NULL,
        [TaxId] NVARCHAR(50) NULL,
        [Notes] NVARCHAR(1000) NULL,
        [IsActive] BIT NOT NULL,
        [RegisteredDate] DATETIME2 NOT NULL,
        [LastPurchaseDate] DATETIME2 NULL,
        [TotalPurchases] DECIMAL(18,2) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL,
        [UpdatedAt] DATETIME2 NOT NULL,
        CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UC_Customers_CustomerCode] UNIQUE ([CustomerCode])
    );
    PRINT 'Customers table created successfully.';
END
ELSE
    PRINT 'Customers table already exists.';
GO

-- =====================================================
-- 9. CustomerInvoices Table
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CustomerInvoices' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[CustomerInvoices] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [InvoiceNumber] NVARCHAR(50) NOT NULL,
        [CustomerId] INT NOT NULL,
        [InvoiceDate] DATETIME2 NOT NULL,
        [DueDate] DATETIME2 NOT NULL,
        [SubTotal] DECIMAL(18,2) NOT NULL,
        [TaxAmount] DECIMAL(18,2) NOT NULL,
        [DiscountAmount] DECIMAL(18,2) NOT NULL,
        [TotalAmount] DECIMAL(18,2) NOT NULL,
        [PaidAmount] DECIMAL(18,2) NOT NULL,
        [Status] NVARCHAR(20) NOT NULL,
        [PaymentTerms] NVARCHAR(100) NULL,
        [Notes] NVARCHAR(500) NULL,
        [CreatedByUserId] INT NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL,
        [UpdatedAt] DATETIME2 NOT NULL,
        [IsActive] BIT NOT NULL,
        CONSTRAINT [PK_CustomerInvoices] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UC_CustomerInvoices_InvoiceNumber] UNIQUE ([InvoiceNumber]),
        CONSTRAINT [FK_CustomerInvoices_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers]([Id]),
        CONSTRAINT [FK_CustomerInvoices_Users] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users]([Id])
    );
    PRINT 'CustomerInvoices table created successfully.';
END
ELSE
    PRINT 'CustomerInvoices table already exists.';
GO

-- =====================================================
-- 10. CustomerInvoiceItems Table
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CustomerInvoiceItems' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[CustomerInvoiceItems] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [InvoiceId] INT NOT NULL,
        [ProductId] INT NOT NULL,
        [Quantity] INT NOT NULL,
        [UnitPrice] DECIMAL(18,2) NOT NULL,
        [DiscountPercentage] DECIMAL(5,2) NOT NULL,
        [TaxPercentage] DECIMAL(5,2) NOT NULL,
        [Description] NVARCHAR(200) NULL,
        [CreatedAt] DATETIME2 NOT NULL,
        [UpdatedAt] DATETIME2 NOT NULL,
        [IsActive] BIT NOT NULL,
        CONSTRAINT [PK_CustomerInvoiceItems] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_CustomerInvoiceItems_CustomerInvoices] FOREIGN KEY ([InvoiceId]) REFERENCES [dbo].[CustomerInvoices]([Id]),
        CONSTRAINT [FK_CustomerInvoiceItems_Products] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products]([Id])
    );
    PRINT 'CustomerInvoiceItems table created successfully.';
END
ELSE
    PRINT 'CustomerInvoiceItems table already exists.';
GO

-- =====================================================
-- 11. CustomerPayments Table
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CustomerPayments' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[CustomerPayments] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [PaymentNumber] NVARCHAR(50) NOT NULL,
        [CustomerId] INT NOT NULL,
        [InvoiceId] INT NULL,
        [PaymentDate] DATETIME2 NOT NULL,
        [Amount] DECIMAL(18,2) NOT NULL,
        [PaymentMethod] NVARCHAR(50) NOT NULL,
        [PaymentType] NVARCHAR(20) NOT NULL,
        [ReferenceNumber] NVARCHAR(100) NULL,
        [Notes] NVARCHAR(500) NULL,
        [RecordedByUserId] INT NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL,
        [UpdatedAt] DATETIME2 NOT NULL,
        [IsActive] BIT NOT NULL,
        CONSTRAINT [PK_CustomerPayments] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UC_CustomerPayments_PaymentNumber] UNIQUE ([PaymentNumber]),
        CONSTRAINT [FK_CustomerPayments_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers]([Id]),
        CONSTRAINT [FK_CustomerPayments_CustomerInvoices] FOREIGN KEY ([InvoiceId]) REFERENCES [dbo].[CustomerInvoices]([Id]),
        CONSTRAINT [FK_CustomerPayments_Users] FOREIGN KEY ([RecordedByUserId]) REFERENCES [dbo].[Users]([Id])
    );
    PRINT 'CustomerPayments table created successfully.';
END
ELSE
    PRINT 'CustomerPayments table already exists.';
GO

PRINT 'All tables created successfully!';