WITH CurrentInventory AS (
SELECT Inventory.ASIN,
	   Inventory.ProductName,
	   Inventory.AfnFulfillableQuantity
FROM(
SELECT uid.ASIN,
	   uid.[product-name] as ProductName,
	   uid.[afn-fulfillable-quantity] as AfnFulfillableQuantity,
	   ROW_NUMBER() OVER(PARTITION BY uid.ASIN ORDER BY uid.[afn-fulfillable-quantity] DESC) as RowNumber
  FROM [AmazonMWS].[dbo].[UnsuppressedInventoryData] uid
  WHERE uid.Date = (SELECT MAX(Date) FROM [AmazonMWS].[dbo].[UnsuppressedInventoryData] uid2)) Inventory
WHERE Inventory.RowNumber = 1 AND Inventory.AfnFulfillableQuantity > 0
), 

SELECT *
FROM CurrentInventory