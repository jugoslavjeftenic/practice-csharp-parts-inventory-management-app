SELECT 
    [PartId],
    [PartCategory],
    [PartName],
    ISNULL([PartQuantity], 0) AS PartQuantity
FROM dbo.Parts
ORDER BY PartId DESC
GO

SELECT 
    [LocationId],
    [LocationAlpha],
    [LocationName],
    [LocationColor]
FROM dbo.Locations
ORDER BY LocationId DESC
GO
