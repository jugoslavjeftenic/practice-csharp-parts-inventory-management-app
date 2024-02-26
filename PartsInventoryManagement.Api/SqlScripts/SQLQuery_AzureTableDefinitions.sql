CREATE TABLE PartCategories
(
    PartCategoryId INT IDENTITY(1,1) PRIMARY KEY,
    PartCategoryName NVARCHAR(50)
)
GO

CREATE TABLE Parts
(
    PartId INT IDENTITY(1,1) PRIMARY KEY,
    PartName NVARCHAR(MAX),
    PartCategoryId INT,
    CONSTRAINT FK_Parts_PartCategories FOREIGN KEY (PartCategoryId) REFERENCES PartCategories(PartCategoryId)
)
GO

CREATE TABLE Locations
(
    LocationId INT IDENTITY(1,1) PRIMARY KEY,
    LocationAlpha VARCHAR(5),
    LocationName NVARCHAR(MAX),
    LocationHexColor VARCHAR(6)
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
