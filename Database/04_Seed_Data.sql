-- =====================================================
-- Inventory Management System - Initial Seed Data
-- SQL Server 14 Compatible
-- =====================================================

USE [InventoryManagementDB];
GO

PRINT 'Inserting seed data...';

-- =====================================================
-- 1. Categories Seed Data
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Categories])
BEGIN
    INSERT INTO [dbo].[Categories] ([Name], [Description], [IsActive])
    VALUES 
        ('Electronics', 'Electronic devices and components', 1),
        ('Clothing', 'Apparel and fashion items', 1),
        ('Books', 'Books and educational materials', 1),
        ('Home & Garden', 'Home improvement and garden supplies', 1),
        ('Sports & Outdoors', 'Sports equipment and outdoor gear', 1),
        ('Automotive', 'Car parts and automotive supplies', 1),
        ('Health & Beauty', 'Health, beauty, and personal care products', 1),
        ('Office Supplies', 'Office equipment and stationery', 1),
        ('Tools & Hardware', 'Tools and hardware supplies', 1),
        ('Food & Beverages', 'Food items and beverages', 1);
        
    PRINT 'Categories seed data inserted.';
END
ELSE
    PRINT 'Categories already contain data.';
GO

-- =====================================================
-- 2. Warehouses Seed Data
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Warehouses])
BEGIN
    INSERT INTO [dbo].[Warehouses] ([Name], [Location], [Address], [ContactPhone], [ContactEmail], [Capacity], [IsActive])
    VALUES 
        ('Main Warehouse', 'Downtown', '123 Main Street, City Center, State 12345', '+1-555-0101', 'main@warehouse.com', 10000, 1),
        ('North Distribution Center', 'North District', '456 North Avenue, North District, State 12346', '+1-555-0102', 'north@warehouse.com', 7500, 1),
        ('South Storage Facility', 'South District', '789 South Boulevard, South District, State 12347', '+1-555-0103', 'south@warehouse.com', 5000, 1),
        ('East Regional Hub', 'East Side', '321 East Road, East Side, State 12348', '+1-555-0104', 'east@warehouse.com', 8000, 1),
        ('West Coast Depot', 'West District', '654 West Street, West District, State 12349', '+1-555-0105', 'west@warehouse.com', 6000, 1);
        
    PRINT 'Warehouses seed data inserted.';
END
ELSE
    PRINT 'Warehouses already contain data.';
GO

-- =====================================================
-- 3. Suppliers Seed Data
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Suppliers])
BEGIN
    INSERT INTO [dbo].[Suppliers] ([Name], [ContactInfo], [Address], [Phone], [Email], [Website], [PaymentTerms], [IsActive])
    VALUES 
        ('TechSupplier Co.', 'John Smith, Purchasing Manager', '100 Tech Park, Silicon Valley, CA 94000', '+1-800-TECH-001', 'orders@techsupplier.com', 'www.techsupplier.com', 'Net 30', 1),
        ('Fashion Forward Ltd.', 'Jane Doe, Sales Director', '200 Fashion Ave, New York, NY 10001', '+1-800-FASHION-1', 'sales@fashionforward.com', 'www.fashionforward.com', 'Net 15', 1),
        ('Book Distributors Inc.', 'Mike Johnson, Distribution Head', '300 Library Lane, Boston, MA 02101', '+1-800-BOOKS-01', 'orders@bookdist.com', 'www.bookdistributors.com', 'Net 45', 1),
        ('Home & Garden Supply', 'Sarah Wilson, Account Manager', '400 Garden Road, Portland, OR 97201', '+1-800-GARDEN-1', 'wholesale@homegardens.com', 'www.homegardens.com', 'Net 30', 1),
        ('Sports Gear Wholesale', 'Tom Anderson, Regional Manager', '500 Sports Complex, Denver, CO 80201', '+1-800-SPORTS-1', 'orders@sportsgear.com', 'www.sportsgearwholesale.com', 'Net 30', 1),
        ('Auto Parts Direct', 'Lisa Brown, Procurement', '600 Industrial Blvd, Detroit, MI 48201', '+1-800-AUTO-001', 'wholesale@autoparts.com', 'www.autopartsdirect.com', 'Net 30', 1),
        ('Global Office Solutions', 'David Lee, B2B Sales', '700 Corporate Center, Chicago, IL 60601', '+1-800-OFFICE-1', 'b2b@globaloffice.com', 'www.globalofficesolutions.com', 'Net 30', 1),
        ('Industrial Tools Corp.', 'Rachel Green, Industrial Sales', '800 Manufacturing Ave, Cleveland, OH 44101', '+1-800-TOOLS-01', 'industrial@toolscorp.com', 'www.industrialtools.com', 'Net 30', 1);
        
    PRINT 'Suppliers seed data inserted.';
END
ELSE
    PRINT 'Suppliers already contain data.';
GO

-- =====================================================
-- 4. Users Seed Data (Admin and Sample Users)
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users])
BEGIN
    -- Note: In production, these passwords should be properly hashed
    -- These are sample hashed passwords for demonstration
    INSERT INTO [dbo].[Users] ([Username], [Email], [PasswordHash], [FullName], [Role], [PhoneNumber], [IsActive])
    VALUES 
        ('admin', 'admin@inventoryms.com', 'AQAAAAEAACcQAAAAEJ4/WHJhBvs3G7l4Zl5xXy+wLj3QWgTqrKdWgjKEPQaJNQx8eLnmZPzXGWKKYjGYZw==', 'System Administrator', 'Administrator', '+1-555-0001', 1),
        ('manager1', 'manager1@inventoryms.com', 'AQAAAAEAACcQAAAAEJ4/WHJhBvs3G7l4Zl5xXy+wLj3QWgTqrKdWgjKEPQaJNQx8eLnmZPzXGWKKYjGYZw==', 'John Manager', 'Manager', '+1-555-0002', 1),
        ('manager2', 'manager2@inventoryms.com', 'AQAAAAEAACcQAAAAEJ4/WHJhBvs3G7l4Zl5xXy+wLj3QWgTqrKdWgjKEPQaJNQx8eLnmZPzXGWKKYjGYZw==', 'Jane Manager', 'Manager', '+1-555-0003', 1),
        ('staff1', 'staff1@inventoryms.com', 'AQAAAAEAACcQAAAAEJ4/WHJhBvs3G7l4Zl5xXy+wLj3QWgTqrKdWgjKEPQaJNQx8eLnmZPzXGWKKYjGYZw==', 'Bob Staff', 'Staff', '+1-555-0004', 1),
        ('staff2', 'staff2@inventoryms.com', 'AQAAAAEAACcQAAAAEJ4/WHJhBvs3G7l4Zl5xXy+wLj3QWgTqrKdWgjKEPQaJNQx8eLnmZPzXGWKKYjGYZw==', 'Alice Staff', 'Staff', '+1-555-0005', 1);
    
    PRINT 'Users seed data inserted.';
    PRINT 'DEFAULT LOGIN: username=admin, password=Admin@123 (Remember to change in production!)';
END
ELSE
    PRINT 'Users already contain data.';
GO

-- =====================================================
-- 5. Sample Products Seed Data
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products])
BEGIN
    DECLARE @ElectronicsId INT = (SELECT Id FROM Categories WHERE Name = 'Electronics');
    DECLARE @ClothingId INT = (SELECT Id FROM Categories WHERE Name = 'Clothing');
    DECLARE @BooksId INT = (SELECT Id FROM Categories WHERE Name = 'Books');
    DECLARE @OfficeId INT = (SELECT Id FROM Categories WHERE Name = 'Office Supplies');
    DECLARE @ToolsId INT = (SELECT Id FROM Categories WHERE Name = 'Tools & Hardware');
    
    DECLARE @TechSupplierId INT = (SELECT Id FROM Suppliers WHERE Name = 'TechSupplier Co.');
    DECLARE @FashionSupplierId INT = (SELECT Id FROM Suppliers WHERE Name = 'Fashion Forward Ltd.');
    DECLARE @BookSupplierId INT = (SELECT Id FROM Suppliers WHERE Name = 'Book Distributors Inc.');
    DECLARE @OfficeSupplierId INT = (SELECT Id FROM Suppliers WHERE Name = 'Global Office Solutions');
    DECLARE @ToolsSupplierId INT = (SELECT Id FROM Suppliers WHERE Name = 'Industrial Tools Corp.');

    INSERT INTO [dbo].[Products] ([Name], [SKU], [Description], [Price], [Cost], [CategoryId], [SupplierId], [LowStockThreshold], [Unit], [Barcode])
    VALUES 
        -- Electronics
        ('Wireless Mouse', 'ELEC-WM-001', 'Ergonomic wireless mouse with USB receiver', 29.99, 18.50, @ElectronicsId, @TechSupplierId, 20, 'Piece', '1234567890123'),
        ('Bluetooth Keyboard', 'ELEC-KB-001', 'Wireless Bluetooth keyboard compatible with multiple devices', 59.99, 35.00, @ElectronicsId, @TechSupplierId, 15, 'Piece', '1234567890124'),
        ('USB-C Cable', 'ELEC-CB-001', '6ft USB-C to USB-C cable for charging and data transfer', 14.99, 8.75, @ElectronicsId, @TechSupplierId, 50, 'Piece', '1234567890125'),
        ('Laptop Stand', 'ELEC-LS-001', 'Adjustable aluminum laptop stand with cooling', 49.99, 28.00, @ElectronicsId, @TechSupplierId, 10, 'Piece', '1234567890126'),
        
        -- Clothing
        ('Cotton T-Shirt', 'CLTH-TS-001', 'Premium cotton crew neck t-shirt - Various sizes', 19.99, 12.00, @ClothingId, @FashionSupplierId, 25, 'Piece', '2234567890123'),
        ('Denim Jeans', 'CLTH-JN-001', 'Classic fit denim jeans - Various sizes and colors', 49.99, 30.00, @ClothingId, @FashionSupplierId, 20, 'Piece', '2234567890124'),
        ('Hoodie Sweatshirt', 'CLTH-HD-001', 'Comfortable hoodie sweatshirt with pocket', 39.99, 24.00, @ClothingId, @FashionSupplierId, 15, 'Piece', '2234567890125'),
        
        -- Books
        ('Business Management 101', 'BOOK-BM-001', 'Comprehensive guide to business management principles', 34.99, 20.00, @BooksId, @BookSupplierId, 10, 'Piece', '3234567890123'),
        ('Programming Fundamentals', 'BOOK-PF-001', 'Introduction to programming concepts and languages', 42.99, 25.50, @BooksId, @BookSupplierId, 12, 'Piece', '3234567890124'),
        ('Project Management Guide', 'BOOK-PM-001', 'Complete project management methodology handbook', 39.99, 23.00, @BooksId, @BookSupplierId, 8, 'Piece', '3234567890125'),
        
        -- Office Supplies
        ('Black Ballpoint Pens', 'OFFC-BP-001', 'Pack of 12 black ballpoint pens', 8.99, 5.25, @OfficeId, @OfficeSupplierId, 30, 'Pack', '4234567890123'),
        ('Letter Paper Ream', 'OFFC-PP-001', 'White 8.5x11 letter paper - 500 sheets', 12.99, 7.50, @OfficeId, @OfficeSupplierId, 40, 'Ream', '4234567890124'),
        ('Stapler Heavy Duty', 'OFFC-ST-001', 'Heavy duty desktop stapler - 25 sheet capacity', 24.99, 14.00, @OfficeId, @OfficeSupplierId, 15, 'Piece', '4234567890125'),
        ('File Folders', 'OFFC-FF-001', 'Manila file folders - Pack of 100', 18.99, 11.00, @OfficeId, @OfficeSupplierId, 25, 'Pack', '4234567890126'),
        
        -- Tools & Hardware
        ('Cordless Drill', 'TOOL-CD-001', '18V cordless drill with battery and charger', 89.99, 52.00, @ToolsId, @ToolsSupplierId, 8, 'Piece', '5234567890123'),
        ('Screwdriver Set', 'TOOL-SS-001', '15-piece precision screwdriver set', 29.99, 17.50, @ToolsId, @ToolsSupplierId, 12, 'Set', '5234567890124'),
        ('Measuring Tape', 'TOOL-MT-001', '25ft heavy duty measuring tape', 19.99, 11.75, @ToolsId, @ToolsSupplierId, 20, 'Piece', '5234567890125'),
        ('Socket Wrench Set', 'TOOL-WS-001', '40-piece socket wrench set with case', 69.99, 41.00, @ToolsId, @ToolsSupplierId, 6, 'Set', '5234567890126');
        
    PRINT 'Products seed data inserted.';
END
ELSE
    PRINT 'Products already contain data.';
GO

-- =====================================================
-- 6. Initial Inventory Seed Data
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Inventory])
BEGIN
    DECLARE @MainWarehouseId INT = (SELECT Id FROM Warehouses WHERE Name = 'Main Warehouse');
    DECLARE @NorthWarehouseId INT = (SELECT Id FROM Warehouses WHERE Name = 'North Distribution Center');
    DECLARE @SouthWarehouseId INT = (SELECT Id FROM Warehouses WHERE Name = 'South Storage Facility');
    
    -- Insert initial inventory for all products across warehouses
    INSERT INTO [dbo].[Inventory] ([ProductId], [WarehouseId], [Quantity], [ReservedQuantity])
    SELECT 
        p.Id as ProductId,
        w.Id as WarehouseId,
        CASE 
            WHEN w.Name = 'Main Warehouse' THEN 100
            WHEN w.Name = 'North Distribution Center' THEN 75
            WHEN w.Name = 'South Storage Facility' THEN 50
            ELSE 25
        END as Quantity,
        0 as ReservedQuantity
    FROM [dbo].[Products] p
    CROSS JOIN (
        SELECT Id, Name FROM [dbo].[Warehouses] 
        WHERE Name IN ('Main Warehouse', 'North Distribution Center', 'South Storage Facility')
    ) w;
    
    PRINT 'Initial inventory seed data inserted.';
END
ELSE
    PRINT 'Inventory already contains data.';
GO

-- =====================================================
-- 7. Sample Transactions Seed Data
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Transactions])
BEGIN
    DECLARE @AdminUserId INT = (SELECT Id FROM Users WHERE Username = 'admin');
    DECLARE @MainWarehouseId INT = (SELECT Id FROM Warehouses WHERE Name = 'Main Warehouse');
    DECLARE @MouseProductId INT = (SELECT Id FROM Products WHERE SKU = 'ELEC-WM-001');
    DECLARE @KeyboardProductId INT = (SELECT Id FROM Products WHERE SKU = 'ELEC-KB-001');
    
    -- Sample initial stock-in transactions
    INSERT INTO [dbo].[Transactions] ([ProductId], [WarehouseId], [UserId], [TransactionType], [QuantityChanged], [PreviousQuantity], [NewQuantity], [UnitCost], [Reason], [ReferenceNumber], [Timestamp])
    VALUES 
        (@MouseProductId, @MainWarehouseId, @AdminUserId, 'StockIn', 100, 0, 100, 18.50, 'Initial inventory setup', 'INV-SETUP-001', DATEADD(DAY, -30, GETUTCDATE())),
        (@KeyboardProductId, @MainWarehouseId, @AdminUserId, 'StockIn', 100, 0, 100, 35.00, 'Initial inventory setup', 'INV-SETUP-002', DATEADD(DAY, -30, GETUTCDATE()));
    
    PRINT 'Sample transactions seed data inserted.';
END
ELSE
    PRINT 'Transactions already contain data.';
GO

-- =====================================================
-- 8. Customers Seed Data
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Customers])
BEGIN
    INSERT INTO [dbo].[Customers] ([CustomerCode], [FullName], [CompanyName], [Email], [PhoneNumber], [Address], [CustomerType], [Balance], [CreditLimit], [PaymentTermsDays], [TaxId], [Notes], [IsActive], [RegisteredDate], [LastPurchaseDate], [TotalPurchases], [CreatedAt], [UpdatedAt])
    VALUES
        ('CUST-001', 'Alice Johnson', 'Johnson Trading', 'alice.johnson@email.com', '+1-555-1010', '101 Main St, City', 'Retail', 0.00, 5000.00, 30, 'TAX12345', 'First customer', 1, GETUTCDATE(), NULL, 0.00, GETUTCDATE(), GETUTCDATE()),
        ('CUST-002', 'Bob Smith', NULL, 'bob.smith@email.com', '+1-555-2020', '202 Second St, City', 'Wholesale', 0.00, 10000.00, 45, NULL, NULL, 1, GETUTCDATE(), NULL, 0.00, GETUTCDATE(), GETUTCDATE());
    PRINT 'Customers seed data inserted.';
END
ELSE
    PRINT 'Customers already contain data.';
GO

-- =====================================================
-- 9. CustomerInvoices Seed Data
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[CustomerInvoices])
BEGIN
    DECLARE @CustomerId1 INT = (SELECT TOP 1 Id FROM Customers WHERE CustomerCode = 'CUST-001');
    DECLARE @CustomerId2 INT = (SELECT TOP 1 Id FROM Customers WHERE CustomerCode = 'CUST-002');
    DECLARE @AdminUserId INT = (SELECT TOP 1 Id FROM Users WHERE Username = 'admin');
    INSERT INTO [dbo].[CustomerInvoices] ([InvoiceNumber], [CustomerId], [InvoiceDate], [DueDate], [SubTotal], [TaxAmount], [DiscountAmount], [TotalAmount], [PaidAmount], [Status], [PaymentTerms], [Notes], [CreatedByUserId], [CreatedAt], [UpdatedAt], [IsActive])
    VALUES
        ('INV-1001', @CustomerId1, DATEADD(DAY, -10, GETUTCDATE()), DATEADD(DAY, 20, GETUTCDATE()), 100.00, 10.00, 5.00, 105.00, 50.00, 'Partial', 'Net 30', 'First invoice', @AdminUserId, GETUTCDATE(), GETUTCDATE(), 1),
        ('INV-1002', @CustomerId2, DATEADD(DAY, -5, GETUTCDATE()), DATEADD(DAY, 25, GETUTCDATE()), 200.00, 20.00, 0.00, 220.00, 0.00, 'Unpaid', 'Net 45', NULL, @AdminUserId, GETUTCDATE(), GETUTCDATE(), 1);
    PRINT 'CustomerInvoices seed data inserted.';
END
ELSE
    PRINT 'CustomerInvoices already contain data.';
GO

-- =====================================================
-- 10. CustomerInvoiceItems Seed Data
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[CustomerInvoiceItems])
BEGIN
    DECLARE @InvoiceId1 INT = (SELECT TOP 1 Id FROM CustomerInvoices WHERE InvoiceNumber = 'INV-1001');
    DECLARE @InvoiceId2 INT = (SELECT TOP 1 Id FROM CustomerInvoices WHERE InvoiceNumber = 'INV-1002');
    DECLARE @ProductId1 INT = (SELECT TOP 1 Id FROM Products WHERE SKU = 'ELEC-WM-001');
    DECLARE @ProductId2 INT = (SELECT TOP 1 Id FROM Products WHERE SKU = 'ELEC-KB-001');
    INSERT INTO [dbo].[CustomerInvoiceItems] ([InvoiceId], [ProductId], [Quantity], [UnitPrice], [DiscountPercentage], [TaxPercentage], [Description], [CreatedAt], [UpdatedAt], [IsActive])
    VALUES
        (@InvoiceId1, @ProductId1, 2, 29.99, 0.00, 10.00, 'Wireless Mouse', GETUTCDATE(), GETUTCDATE(), 1),
        (@InvoiceId1, @ProductId2, 1, 59.99, 5.00, 10.00, 'Bluetooth Keyboard', GETUTCDATE(), GETUTCDATE(), 1),
        (@InvoiceId2, @ProductId2, 3, 59.99, 0.00, 10.00, 'Bluetooth Keyboard', GETUTCDATE(), GETUTCDATE(), 1);
    PRINT 'CustomerInvoiceItems seed data inserted.';
END
ELSE
    PRINT 'CustomerInvoiceItems already contain data.';
GO

-- =====================================================
-- 11. CustomerPayments Seed Data
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[CustomerPayments])
BEGIN
    DECLARE @PaymentCustomerId1 INT = (SELECT TOP 1 Id FROM Customers WHERE CustomerCode = 'CUST-001');
    DECLARE @PaymentInvoiceId1 INT = (SELECT TOP 1 Id FROM CustomerInvoices WHERE InvoiceNumber = 'INV-1001');
    DECLARE @AdminUserId INT = (SELECT TOP 1 Id FROM Users WHERE Username = 'admin');
    INSERT INTO [dbo].[CustomerPayments] ([PaymentNumber], [CustomerId], [InvoiceId], [PaymentDate], [Amount], [PaymentMethod], [PaymentType], [ReferenceNumber], [Notes], [RecordedByUserId], [CreatedAt], [UpdatedAt], [IsActive])
    VALUES
        ('PAY-0001', @PaymentCustomerId1, @PaymentInvoiceId1, DATEADD(DAY, -5, GETUTCDATE()), 50.00, 'Credit Card', 'Payment', 'REF-001', 'Partial payment for invoice INV-1001', @AdminUserId, GETUTCDATE(), GETUTCDATE(), 1);
    PRINT 'CustomerPayments seed data inserted.';
END
ELSE
    PRINT 'CustomerPayments already contain data.';
GO

-- =====================================================
-- Update Statistics After Data Insert
-- =====================================================
UPDATE STATISTICS [dbo].[Categories];
UPDATE STATISTICS [dbo].[Warehouses];
UPDATE STATISTICS [dbo].[Suppliers];
UPDATE STATISTICS [dbo].[Users];
UPDATE STATISTICS [dbo].[Products];
UPDATE STATISTICS [dbo].[Inventory];
UPDATE STATISTICS [dbo].[Transactions];

PRINT 'All seed data inserted successfully!';
PRINT '';
PRINT '========================================';
PRINT 'DATABASE SETUP COMPLETED SUCCESSFULLY!';
PRINT '========================================';
PRINT 'Database: InventoryManagementDB';
PRINT 'Tables: 7 (Categories, Warehouses, Suppliers, Users, Products, Inventory, Transactions)';
PRINT 'Indexes: 25+ performance indexes created';
PRINT 'Seed Data: Sample data for testing and development';
PRINT '';
PRINT 'Default Admin Login:';
PRINT 'Username: admin';
PRINT 'Password: Admin@123 (CHANGE IN PRODUCTION!)';
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Configure Entity Framework DbContext';
PRINT '2. Create domain entities in code';
PRINT '3. Set up repository pattern';
PRINT '========================================';