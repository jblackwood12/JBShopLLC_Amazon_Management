DECLARE @NumDays INTEGER = 365;
DECLARE @DaysToSellInventory INTEGER = 120;

DECLARE @BeginDate DateTime = GETDATE() - @NumDays;
DECLARE @EndDate DateTime = GETDATE();

	WITH DistinctItems AS (
	SELECT AllItems.sku,
		   AllItems.asin,
		AllItems.[product-name],
		AllItems.[afn-warehouse-quantity],
		AllItems.[afn-fulfillable-quantity],
		AllItems.[afn-unsellable-quantity],
		AllItems.[afn-reserved-quantity]
	FROM(SELECT *,
			ROW_NUMBER() OVER (PARTITION BY uia.asin ORDER BY uia.[your-price] ASC) as RowNumber
		   FROM AmazonMWS.dbo.UnsuppressedInventoryData as uia
		  WHERE Date = (SELECT MAX(Date)
	FROM AmazonMWS.dbo.UnsuppressedInventoryData) AND [afn-warehouse-quantity] > 0) AllItems
	WHERE AllItems.RowNumber = 1
	), RecentSales AS (
	  SELECT asin,
			 SUM(aod.[item-price]) as TotalSalesPrice,
			 SUM(aod.[quantity]) as TotalSalesQuantity
	  FROM AmazonMWS.dbo.AllOrdersData aod
	  WHERE CAST(aod.[purchase-date] as Date) >= @BeginDate
	        AND CAST(aod.[purchase-date] as Date) <= @EndDate
			AND aod.[item-status] != 'Cancelled'
	  GROUP BY aod.asin
	), InventoryHistory AS (
	SELECT uid.ASIN,
	       COUNT(DISTINCT(uid.Date)) as DistinctDaysInStock
	  FROM AmazonMWS.dbo.UnsuppressedInventoryData uid
	 WHERE uid.Date >= @BeginDate
	       AND uid.Date <= @EndDate
		   AND uid.[afn-fulfillable-quantity] > 0
	GROUP BY uid.ASIN
	), InventoryLiquidity AS (
	SELECT di.asin,
		   di.sku,
		   di.[product-name],
		   di.[afn-fulfillable-quantity],
		   i.Cost,
		   (di.[afn-fulfillable-quantity] * i.Cost) as TotalInventoryCost,
		   COALESCE(rs.TotalSalesPrice, 0) AS TotalSalesPrice,
		   COALESCE(rs.TotalSalesQuantity, 0) AS TotalSalesQuantity,
		   ih.DistinctDaysInStock,
		   COALESCE(CASE WHEN DistinctDaysInStock != 0 THEN CAST( TotalSalesQuantity as decimal) / DistinctDaysInStock END, 0) AS QuantitySoldPerDay
			  FROM DistinctItems as di
		   JOIN AmazonMWS.dbo.Products as i on i.ASIN = di.asin
		LEFT JOIN RecentSales rs on rs.ASIN = di.ASIN
		LEFT JOIN InventoryHistory ih on ih.ASIN = di.ASIN
			 WHERE di.[afn-fulfillable-quantity] > 0
	)
SELECT SUM(TotalRemainingInventoryCost) AS TotalInventoryCostProject, SUM(TotalInventoryCost) AS TotalCurrentInventoryCost
FROM(
SELECT RemainingInventory.*,
       RemainingInventory.RemainingQuantity * RemainingInventory.Cost as TotalRemainingInventoryCost
FROM (SELECT SimulatedSales.*,
		   CASE WHEN SimulatedSales.ExpectedToSellQuantity > SimulatedSales.[afn-fulfillable-quantity] THEN 0 ELSE SimulatedSales.[afn-fulfillable-quantity] - SimulatedSales.ExpectedToSellQuantity END AS RemainingQuantity
	FROM(
		SELECT il.*,
			   COALESCE(CASE WHEN (@DaysToSellInventory * il.QuantitySoldPerDay) < il.[afn-fulfillable-quantity] THEN (@DaysToSellInventory * il.QuantitySoldPerDay) ELSE il.[afn-fulfillable-quantity] END, 0) AS ExpectedToSellQuantity
		FROM InventoryLiquidity il
		) SimulatedSales) RemainingInventory
		) FinalInventory