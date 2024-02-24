
USE PartsInventory;
GO

SELECT
    [InventoryId],
    [PartId],
    [LocationId],
    ISNULL([PartQuantity], 0) AS PartQuantity
FROM dbo.Inventory
ORDER BY InventoryId DESC
GO

SELECT
    [LocationId],
    [LocationAlpha],
    [LocationName],
    [LocationHexColor]
FROM dbo.Locations
ORDER BY LocationId DESC
GO

SELECT
    [PartCategoryId],
    [PartCategoryName]
FROM dbo.PartCategories
ORDER BY PartCategoryId DESC
GO

SELECT
    [PartId],
    [PartCategoryId],
    [PartName]
FROM dbo.Parts
ORDER BY PartId DESC
GO

SELECT
    [UserId],
    [UserName],
    [LocationId],
    [PasswordHash],
    [PasswordSalt]
FROM dbo.Users
ORDER BY UserId DESC
GO
