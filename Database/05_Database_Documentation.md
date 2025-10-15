# Inventory Management System - Database Schema Documentation

## Overview
This document describes the database schema for the Inventory Management System built on SQL Server 14. The schema follows normalized database design principles and supports Clean Architecture patterns.

## Database Information
- **Name**: InventoryManagementDB
- **Version**: SQL Server 14 compatible
- **Character Set**: UTF-8 (NVARCHAR)
- **Recovery Model**: Full

## Tables Overview

### 1. Categories
**Purpose**: Product categorization and organization

| Column | Data Type | Constraints | Description |
|--------|-----------|-------------|-------------|
| Id | INT IDENTITY(1,1) | PRIMARY KEY | Unique category identifier |
| Name | NVARCHAR(100) | NOT NULL, UNIQUE | Category name |
| Description | NVARCHAR(500) | NULL | Category description |
| CreatedAt | DATETIME2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |
| UpdatedAt | DATETIME2 | NOT NULL, DEFAULT GETUTCDATE() | Last update timestamp |
| IsActive | BIT | NOT NULL, DEFAULT 1 | Active status flag |

**Indexes**: Name, IsActive

---

### 2. Warehouses
**Purpose**: Storage location management

| Column | Data Type | Constraints | Description |
|--------|-----------|-------------|-------------|
| Id | INT IDENTITY(1,1) | PRIMARY KEY | Unique warehouse identifier |
| Name | NVARCHAR(100) | NOT NULL, UNIQUE | Warehouse name |
| Location | NVARCHAR(200) | NOT NULL | General location |
| Address | NVARCHAR(500) | NULL | Full address |
| ContactPhone | NVARCHAR(20) | NULL | Contact phone number |
| ContactEmail | NVARCHAR(100) | NULL | Contact email |
| Capacity | INT | NULL | Storage capacity |
| CreatedAt | DATETIME2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |
| UpdatedAt | DATETIME2 | NOT NULL, DEFAULT GETUTCDATE() | Last update timestamp |
| IsActive | BIT | NOT NULL, DEFAULT 1 | Active status flag |

**Indexes**: Name, IsActive

---

### 3. Suppliers
**Purpose**: Vendor and supplier information management

| Column | Data Type | Constraints | Description |
|--------|-----------|-------------|-------------|
| Id | INT IDENTITY(1,1) | PRIMARY KEY | Unique supplier identifier |
| Name | NVARCHAR(100) | NOT NULL, UNIQUE | Supplier company name |
| ContactInfo | NVARCHAR(200) | NULL | Primary contact information |
| Address | NVARCHAR(500) | NULL | Supplier address |
| Phone | NVARCHAR(20) | NULL | Phone number |
| Email | NVARCHAR(100) | NULL | Email address |
| Website | NVARCHAR(200) | NULL | Company website |
| TaxNumber | NVARCHAR(50) | NULL | Tax identification number |
| PaymentTerms | NVARCHAR(100) | NULL | Payment terms (Net 30, etc.) |
| CreatedAt | DATETIME2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |
| UpdatedAt | DATETIME2 | NOT NULL, DEFAULT GETUTCDATE() | Last update timestamp |
| IsActive | BIT | NOT NULL, DEFAULT 1 | Active status flag |

**Indexes**: Name, IsActive

---

### 4. Users
**Purpose**: System user authentication and authorization

| Column | Data Type | Constraints | Description |
|--------|-----------|-------------|-------------|
| Id | INT IDENTITY(1,1) | PRIMARY KEY | Unique user identifier |
| Username | NVARCHAR(50) | NOT NULL, UNIQUE | Login username |
| Email | NVARCHAR(100) | NOT NULL, UNIQUE | Email address |
| PasswordHash | NVARCHAR(256) | NOT NULL | Hashed password |
| FullName | NVARCHAR(100) | NOT NULL | User's full name |
| Role | NVARCHAR(20) | NOT NULL, CHECK constraint | User role (Administrator, Manager, Staff) |
| PhoneNumber | NVARCHAR(20) | NULL | Phone number |
| LastLoginAt | DATETIME2 | NULL | Last login timestamp |
| CreatedAt | DATETIME2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |
| UpdatedAt | DATETIME2 | NOT NULL, DEFAULT GETUTCDATE() | Last update timestamp |
| IsActive | BIT | NOT NULL, DEFAULT 1 | Active status flag |

**Constraints**: Role must be in ('Administrator', 'Manager', 'Staff')
**Indexes**: Username, Email, Role_IsActive

---

### 5. Products
**Purpose**: Product catalog and master data

| Column | Data Type | Constraints | Description |
|--------|-----------|-------------|-------------|
| Id | INT IDENTITY(1,1) | PRIMARY KEY | Unique product identifier |
| Name | NVARCHAR(100) | NOT NULL | Product name |
| SKU | NVARCHAR(50) | NOT NULL, UNIQUE | Stock Keeping Unit |
| Description | NVARCHAR(1000) | NULL | Product description |
| Price | DECIMAL(18,2) | NOT NULL, CHECK >= 0 | Selling price |
| Cost | DECIMAL(18,2) | NULL, CHECK >= 0 | Cost price |
| CategoryId | INT | NOT NULL, FOREIGN KEY | Reference to Categories |
| SupplierId | INT | NULL, FOREIGN KEY | Reference to Suppliers |
| LowStockThreshold | INT | NOT NULL, DEFAULT 10, CHECK >= 0 | Low stock alert threshold |
| Unit | NVARCHAR(20) | NOT NULL, DEFAULT 'Piece' | Unit of measurement |
| Barcode | NVARCHAR(100) | NULL | Product barcode |
| Weight | DECIMAL(10,3) | NULL | Product weight |
| Dimensions | NVARCHAR(100) | NULL | Product dimensions |
| CreatedAt | DATETIME2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |
| UpdatedAt | DATETIME2 | NOT NULL, DEFAULT GETUTCDATE() | Last update timestamp |
| IsActive | BIT | NOT NULL, DEFAULT 1 | Active status flag |

**Foreign Keys**: 
- CategoryId → Categories(Id)
- SupplierId → Suppliers(Id)

**Indexes**: SKU, Name, CategoryId, SupplierId, IsActive, Barcode

---

### 6. Inventory
**Purpose**: Current stock levels per product per warehouse

| Column | Data Type | Constraints | Description |
|--------|-----------|-------------|-------------|
| Id | INT IDENTITY(1,1) | PRIMARY KEY | Unique inventory record identifier |
| ProductId | INT | NOT NULL, FOREIGN KEY | Reference to Products |
| WarehouseId | INT | NOT NULL, FOREIGN KEY | Reference to Warehouses |
| Quantity | INT | NOT NULL, DEFAULT 0, CHECK >= 0 | Current stock quantity |
| ReservedQuantity | INT | NOT NULL, DEFAULT 0, CHECK >= 0 | Reserved stock |
| AvailableQuantity | AS (Quantity - ReservedQuantity) | PERSISTED COMPUTED | Available for sale |
| LastStockCount | DATETIME2 | NULL | Last physical count date |
| CreatedAt | DATETIME2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |
| UpdatedAt | DATETIME2 | NOT NULL, DEFAULT GETUTCDATE() | Last update timestamp |

**Foreign Keys**: 
- ProductId → Products(Id)
- WarehouseId → Warehouses(Id)

**Unique Constraint**: (ProductId, WarehouseId)
**Check Constraints**: ReservedQuantity <= Quantity
**Indexes**: ProductId, WarehouseId, LowStock (Quantity ASC)

---

### 7. Transactions
**Purpose**: Complete audit trail of all inventory movements

| Column | Data Type | Constraints | Description |
|--------|-----------|-------------|-------------|
| Id | INT IDENTITY(1,1) | PRIMARY KEY | Unique transaction identifier |
| ProductId | INT | NOT NULL, FOREIGN KEY | Reference to Products |
| WarehouseId | INT | NOT NULL, FOREIGN KEY | Reference to Warehouses |
| UserId | INT | NOT NULL, FOREIGN KEY | Reference to Users |
| TransactionType | NVARCHAR(20) | NOT NULL, CHECK constraint | Type of transaction |
| QuantityChanged | INT | NOT NULL, CHECK != 0 | Quantity change (+ or -) |
| PreviousQuantity | INT | NOT NULL, CHECK >= 0 | Stock before transaction |
| NewQuantity | INT | NOT NULL, CHECK >= 0 | Stock after transaction |
| UnitCost | DECIMAL(18,2) | NULL, CHECK >= 0 | Cost per unit |
| TotalValue | AS (QuantityChanged * UnitCost) | PERSISTED COMPUTED | Total transaction value |
| Reason | NVARCHAR(200) | NULL | Reason for transaction |
| ReferenceNumber | NVARCHAR(50) | NULL | External reference number |
| Notes | NVARCHAR(500) | NULL | Additional notes |
| Timestamp | DATETIME2 | NOT NULL, DEFAULT GETUTCDATE() | Transaction timestamp |
| CreatedAt | DATETIME2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |

**Foreign Keys**: 
- ProductId → Products(Id)
- WarehouseId → Warehouses(Id)
- UserId → Users(Id)

**Constraints**: TransactionType IN ('StockIn', 'StockOut', 'Adjustment')
**Indexes**: ProductId_Timestamp, WarehouseId_Timestamp, UserId_Timestamp, TransactionType_Timestamp, Timestamp, ReferenceNumber

## Relationships

### Entity Relationship Diagram (Text Format)
```
Categories ||--o{ Products
Suppliers ||--o{ Products
Products ||--o{ Inventory
Warehouses ||--o{ Inventory
Products ||--o{ Transactions
Warehouses ||--o{ Transactions
Users ||--o{ Transactions
```

### Detailed Relationships
1. **Categories** → **Products** (One-to-Many)
   - One category can have many products
   - Each product belongs to exactly one category

2. **Suppliers** → **Products** (One-to-Many, Optional)
   - One supplier can supply many products
   - Each product can have one primary supplier (optional)

3. **Products** → **Inventory** (One-to-Many)
   - One product can be stored in many warehouses
   - Each inventory record represents one product in one warehouse

4. **Warehouses** → **Inventory** (One-to-Many)
   - One warehouse can store many products
   - Each inventory record belongs to one warehouse

5. **Products** → **Transactions** (One-to-Many)
   - One product can have many transactions
   - Each transaction involves exactly one product

6. **Warehouses** → **Transactions** (One-to-Many)
   - One warehouse can have many transactions
   - Each transaction occurs in exactly one warehouse

7. **Users** → **Transactions** (One-to-Many)
   - One user can create many transactions
   - Each transaction is created by exactly one user

## Performance Optimization

### Indexing Strategy
1. **Primary Keys**: All tables have clustered indexes on Id columns
2. **Foreign Keys**: All foreign key columns have non-clustered indexes
3. **Unique Constraints**: Unique indexes on Name fields and business keys
4. **Query Optimization**: Composite indexes for common query patterns
5. **Low Stock Queries**: Specialized index on Inventory.Quantity
6. **Reporting**: Time-based indexes on Transactions.Timestamp
7. **Search**: Indexes on commonly searched fields (SKU, Barcode, Username)

### Query Performance Features
- **Computed Columns**: AvailableQuantity, TotalValue
- **Covering Indexes**: Include commonly selected columns
- **Filtered Indexes**: Where clauses for active records only
- **Statistics**: Automatic statistics creation and updates enabled

## Business Rules Enforced by Database

1. **Data Integrity**:
   - No negative quantities in inventory
   - Reserved quantity cannot exceed total quantity
   - All prices and costs must be non-negative
   - Transaction quantity changes cannot be zero

2. **Referential Integrity**:
   - Products must belong to valid categories
   - Inventory records must reference valid products and warehouses
   - Transactions must reference valid products, warehouses, and users

3. **Business Logic**:
   - User roles are restricted to predefined values
   - Transaction types are restricted to predefined values
   - Unique constraints prevent duplicate SKUs, usernames, and emails

4. **Audit Trail**:
   - All tables have CreatedAt timestamps
   - Most tables have UpdatedAt timestamps for change tracking
   - Complete transaction history is maintained

## Security Considerations

1. **Password Storage**: Password hashes, not plain text
2. **Soft Deletes**: IsActive flags instead of hard deletes
3. **Audit Trail**: Complete transaction history for compliance
4. **Data Types**: NVARCHAR for Unicode support
5. **Constraints**: Database-level validation for data integrity

## Initial Data

The database includes seed data for:
- **Categories**: 10 common product categories
- **Warehouses**: 5 sample warehouse locations
- **Suppliers**: 8 sample suppliers
- **Users**: Admin and sample users for each role
- **Products**: 17 sample products across categories
- **Inventory**: Initial stock levels for all products
- **Transactions**: Sample initial stock-in transactions

### Default Admin Account
- **Username**: admin
- **Password**: Admin@123 (MUST be changed in production)
- **Role**: Administrator

## Maintenance

### Regular Maintenance Tasks
1. **Statistics Updates**: Automatic updates enabled
2. **Index Maintenance**: Regular index rebuilding recommended
3. **Backup Strategy**: Full recovery model for point-in-time recovery
4. **Archive Strategy**: Consider archiving old transactions for performance
5. **Monitoring**: Track query performance and optimize as needed

### Growth Considerations
- **Partitioning**: Consider table partitioning for Transactions table growth
- **Archiving**: Implement archiving strategy for historical data
- **Scaling**: Horizontal scaling considerations for read operations
- **Caching**: Application-level caching for frequently accessed data

---
