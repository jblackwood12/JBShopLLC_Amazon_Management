WITH InventoryInStock AS (
SELECT *
FROM(
SELECT *,
	ROW_NUMBER() OVER (PARTITION BY uia.asin ORDER BY uia.[your-price] ASC) as RowNumber
FROM AmazonMWS.dbo.UnsuppressedInventoryData as uia
WHERE Date = (SELECT MAX(Date) FROM AmazonMWS.dbo.UnsuppressedInventoryData uia2) AND [afn-warehouse-quantity] > 0) InventoryInStock
WHERE InventoryInStock.RowNumber = 1
)

SELECT p.*,
	   iis.[afn-warehouse-quantity] as QuantityInStock
  FROM [AmazonMWS].[dbo].[Products] p
  JOIN InventoryInStock iis on iis.ASIN = p.ASIN
  WHERE p.ASIN IS NOT NULL
	    AND (Length IS NULL OR Width IS NULL OR Height IS NULL OR Weight IS NULL)