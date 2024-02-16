CREATE DATABASE PartsInventory
GO

USE PartsInventory
GO

CREATE TABLE Parts
(
    PartId INT IDENTITY(1,1) PRIMARY KEY,
    PartCategory NVARCHAR(50),
    PartName NVARCHAR(MAX),
    PartQuantity INT,
)
GO

CREATE TABLE Locations
(
    LocationId INT IDENTITY(1,1) PRIMARY KEY,
    LocationAlpha VARCHAR(5),
    LocationName NVARCHAR(MAX),
    LocationColor VARCHAR(6),
)
GO
