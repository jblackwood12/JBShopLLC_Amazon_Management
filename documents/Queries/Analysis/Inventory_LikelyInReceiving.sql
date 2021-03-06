WITH InventoryBeforeRemovingDuplicates AS (
SELECT uid.*,
       ROW_NUMBER() OVER (PARTITION BY uid.ASIN ORDER BY [afn-warehouse-quantity] DESC) as RowNumber
  FROM [AmazonMWS].[dbo].[UnsuppressedInventoryData] uid
  WHERE (SELECT MAX(uid2.Date) FROM [AmazonMWS].[dbo].[UnsuppressedInventoryData] uid2) = uid.Date
	    AND uid.[afn-warehouse-quantity] > 0
), InventoryThatMayStillBeInReceiving AS (
	SELECT ASIN,
		   SKU,
		   [product-name] as ProductName,
		   ibrfd.[afn-warehouse-quantity] as AfnWarehouseQuantity,
		   ibrfd.[afn-fulfillable-quantity] as AfnFulfillableQuantity,
		   ibrfd.[afn-unsellable-quantity] as AfnUnsellableQuantity,
		   ibrfd.[afn-reserved-quantity] as AfnReservedQuantity
	  FROM InventoryBeforeRemovingDuplicates ibrfd
	 WHERE ibrfd.RowNumber = 1
)


SELECT *
  FROM InventoryThatMayStillBeInReceiving itmsbir
WHERE itmsbir.AfnReservedQuantity > 0
ORDER BY itmsbir.AfnReservedQuantity DESC