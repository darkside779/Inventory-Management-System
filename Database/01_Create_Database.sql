-- =====================================================
-- Inventory Management System - Database Creation
-- SQL Server 14 Compatible
-- =====================================================

-- Create the database
IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'InventoryManagementDB')
BEGIN
    CREATE DATABASE [InventoryManagementDB];
    PRINT 'Database InventoryManagementDB created successfully.';
END
ELSE
BEGIN
    PRINT 'Database InventoryManagementDB already exists.';
END
GO

-- Switch to the new database
USE [InventoryManagementDB];
GO

-- Set database options for optimal performance
ALTER DATABASE [InventoryManagementDB] SET RECOVERY FULL;
ALTER DATABASE [InventoryManagementDB] SET AUTO_SHRINK OFF;
ALTER DATABASE [InventoryManagementDB] SET AUTO_CREATE_STATISTICS ON;
ALTER DATABASE [InventoryManagementDB] SET AUTO_UPDATE_STATISTICS ON;
GO

PRINT 'Database setup completed successfully.';