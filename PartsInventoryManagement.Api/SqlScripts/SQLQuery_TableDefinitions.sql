--CREATE DATABASE PartsInventory
--DROP DATABASE PartsInventory
GO

USE PartsInventory;
GO

IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL
BEGIN
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Users_Locations')
    BEGIN
        ALTER TABLE dbo.Inventory DROP CONSTRAINT FK_Users_Locations;
    END;

    DROP TABLE dbo.Users;
END;
GO

IF OBJECT_ID('dbo.Inventory', 'U') IS NOT NULL
BEGIN
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Inventory_Parts')
    BEGIN
        ALTER TABLE dbo.Inventory DROP CONSTRAINT FK_Inventory_Parts;
    END;

    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Inventory_Locations')
    BEGIN
        ALTER TABLE dbo.Inventory DROP CONSTRAINT FK_Inventory_Locations;
    END;

    DROP TABLE dbo.Inventory;
END;
GO

IF OBJECT_ID('dbo.Locations', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Locations;
END;
GO

IF OBJECT_ID('dbo.Parts', 'U') IS NOT NULL
BEGIN
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Parts_PartCategories')
    BEGIN
        ALTER TABLE dbo.Parts DROP CONSTRAINT FK_Parts_PartCategories;
    END;

    DROP TABLE dbo.Parts;
END;
GO

IF OBJECT_ID('dbo.PartCategories', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.PartCategories;
END;
GO

CREATE TABLE PartCategories
(
    PartCategoryId INT IDENTITY(1,1) PRIMARY KEY,
    PartCategoryName NVARCHAR(50)
)
GO

CREATE TABLE Parts
(
    PartId INT IDENTITY(1,1) PRIMARY KEY,
    PartCategoryId INT,
    PartName NVARCHAR(MAX),
    CONSTRAINT FK_Parts_PartCategories FOREIGN KEY (PartCategoryId) REFERENCES PartCategories(PartCategoryId)
)
GO

CREATE TABLE Locations
(
    LocationId INT IDENTITY(1,1) PRIMARY KEY,
    LocationAlpha VARCHAR(5),
    LocationName NVARCHAR(MAX),
    LocationColor VARCHAR(6)
)
GO

CREATE TABLE Inventory
(
    InventoryId INT IDENTITY(1,1) PRIMARY KEY,
    PartId INT,
    LocationId INT,
    PartQuantity INT,
    CONSTRAINT FK_Inventory_Parts FOREIGN KEY (PartId) REFERENCES Parts(PartId),
    CONSTRAINT FK_Inventory_Locations FOREIGN KEY (LocationId) REFERENCES Locations(LocationId)
)
GO

CREATE TABLE Users
(
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(20),
    LocationId INT,
	PasswordHash VARBINARY(MAX) NULL,
	PasswordSalt VARBINARY(MAX) NULL
    CONSTRAINT FK_Users_Locations FOREIGN KEY (LocationId) REFERENCES Locations(LocationId)
)
GO
