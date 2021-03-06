DECLARE @BeginDate DateTime = DATEADD(DAY, -730, GETDATE());
DECLARE @EndDate DateTime = GETDATE();
DECLARE @DaysToReorderFor INT = 120;
DECLARE @ManufacturerId INT = 1;

WITH ProductsToReorder AS (
 SELECT p.ASIN, p.Name, p.Cost, p.ItemNumber, p.SKU
   FROM dbo.Products p
  WHERE ManufacturerId = @ManufacturerId
		AND p.IsDiscontinued = 0
), Sales AS (
SELECT p.ASIN,
	   SUM(aod.quantity) as TotalSold365Days
  FROM ProductsToReorder p
  JOIN dbo.AllOrdersData aod on aod.ASIN = p.ASIN
 WHERE aod.[purchase-date] >= @BeginDate
	   AND aod.[purchase-date] <= @EndDate
	   AND aod.[order-status] != 'Cancelled'
GROUP BY p.ASIN
), Inventory AS (
SELECT *
FROM(
SELECT p.ASIN,
	   uid.[afn-fulfillable-quantity] as AfnFulfillableQuantity,
	   ROW_NUMBER() OVER(PARTITION BY uid.ASIN ORDER BY uid.[your-price] ASC) as RowNumber
  FROM dbo.UnsuppressedInventoryData uid
  JOIN ProductsToReorder p on p.ASIN = uid.ASIN
  WHERE uid.Date = (SELECT MAX(Date) FROM dbo.UnsuppressedInventoryData uid)) CurrentInventory
  WHERE CurrentInventory.RowNumber = 1
), DaysInStock AS (
	SELECT uid.ASIN, COUNT(DISTINCT(uid.Date)) as DaysInStock
	FROM dbo.UnsuppressedInventoryData uid
	JOIN ProductsToReorder ptr on ptr.ASIN = uid.ASIN
	WHERE uid.date >= @BeginDate
		   AND uid.date <= @EndDate
		   AND uid.[afn-fulfillable-quantity] > 0
	GROUP BY uid.ASIN
)

SELECT SuggestedReorderQuantity.ASIN,
	   SuggestedReorderQuantity.SKU,
       SuggestedReorderQuantity.Name,
	   SuggestedReorderQuantity.ItemNumber,
	   SuggestedReorderQuantity.Cost,
	   SuggestedReorderQuantity.TotalSold365Days,
	   SuggestedReorderQuantity.AfnFulfillableQuantity,
	   SuggestedReorderQuantity.DaysInStock,
	   SuggestedReorderQuantity.UnitsSoldPerDayInStock,
	   FORMAT(CASE WHEN SuggestedReorderQuantity.ReorderQuantity > 0 THEN SuggestedReorderQuantity.ReorderQuantity ELSE 0 END, 'N2') as ReorderQuantity
FROM(
SELECT SalesData.ASIN,
		SalesData.SKU,
       SalesData.Name,
	   SalesData.ItemNumber,
	   SalesData.Cost,
	   SalesData.TotalSold365Days,
	   SalesData.AfnFulfillableQuantity,
	   SalesData.DaysInStock,
	   FORMAT(SalesData.UnitsSoldPerDayInStock, 'N2') as UnitsSoldPerDayInStock,
	   ((@DaysToReorderFor * SalesData.UnitsSoldPerDayInStock) - COALESCE(SalesData.AfnFulfillableQuantity,0)) AS ReorderQuantity
FROM(
SELECT ptr.ASIN,
	   ptr.SKU,
       ptr.Name,
	   ptr.ItemNumber,
	   ptr.Cost,
	   s.TotalSold365Days,
	   i.AfnFulfillableQuantity,
	   dis.DaysInStock,
	   CASE WHEN s.TotalSold365Days > 0 THEN s.TotalSold365Days / CAST(dis.DaysInStock as decimal) ELSE NULL END AS UnitsSoldPerDayInStock
FROM ProductsToReorder ptr
LEFT JOIN Sales s on s.ASIN = ptr.ASIN
LEFT JOIN Inventory i on i.ASIN = ptr.ASIN
LEFT JOIN DaysInStock as dis on dis.ASIN = ptr.ASIN) SalesData) SuggestedReorderQuantity
WHERE SuggestedReorderQuantity.ReorderQuantity > 0
ORDER BY SuggestedReorderQuantity.ReorderQuantity DESC