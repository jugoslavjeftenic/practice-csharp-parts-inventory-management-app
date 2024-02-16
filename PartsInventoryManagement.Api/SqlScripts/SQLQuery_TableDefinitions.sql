-- CREATE DATABASE PartsInventory
-- GO

USE PartsInventory;
GO

IF OBJECT_ID('dbo.Inventory', 'U') IS NOT NULL
BEGIN
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
    CONSTRAINT FK_Parts_PartCategories FOREIGN KEY (PartId) REFERENCES PartCategories(PartCategoryId),
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
